using DG.Tweening;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject arrowPrefab;

    [Header("Fire Settings")]
    [SerializeField] private float attackInterval = 0.6f;
    [SerializeField] private float idleEps = 0.02f;

    [Header("Trajectory Common")]
    [SerializeField] private float projectileSpeed = 12f;   // m/s (t=0→1 구간 소요시간을 계산)
    [SerializeField] private float collisionDelay = 0.1f;   // 발사 직후 충돌 무시 시간(옵션)

    [Header("Parabola (Bezier)")]
    [SerializeField] private float arcMin = 0.8f;
    [SerializeField] private float arcMax = 3.5f;
    [SerializeField] private float arcAtMaxRange = 12f;
    [SerializeField] private AnimationCurve arcCurve = AnimationCurve.Linear(0,0,1,1);

    // t>1로 얼마나 더 갈지(상한). 충돌이 나면 그 전에 끝난다.
    [SerializeField] private float tMaxBeyond = 10f;

    private Rigidbody2D rb;
    private float timer;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        if (!muzzle) muzzle = transform;
    }

    private void Update()
    {
        if (!enemy || !arrowPrefab) return;

        timer += Time.deltaTime;
        bool isIdle = rb ? Mathf.Abs(rb.velocity.x) <= idleEps : true;

        if (isIdle && timer >= attackInterval)
        {
            FireOnce();
            timer = 0f;
        }
    }

    private void FireOnce()
    {
        // --- 스냅샷 ---
        Vector2 S = muzzle.position;
        Vector2 E = enemy.position;

        // --- 호 높이 계산(적까지 거리 기준) ---
        float distToTarget = Vector2.Distance(S, E);
        float tNorm = Mathf.Clamp01(distToTarget / Mathf.Max(0.001f, arcAtMaxRange));
        float arcH  = Mathf.Lerp(arcMin, arcMax, arcCurve.Evaluate(tNorm));

        // --- 컨트롤 포인트 C 계산 ---
        Vector2 v = E - S;
        Vector2 n = Vector2.up;   // 디폴트
        Vector2 M = (S + E) * 0.5f;
        Vector2 C = M + 2f * arcH * n;   // C = M + 2*d

        // --- t=0→1 구간 소요시간(대략 chord 길이로 근사) ---
        float duration01 = Mathf.Max(0.05f, distToTarget / Mathf.Max(0.001f, projectileSpeed));

        // --- 투사체 생성/무장 ---
        var go     = Instantiate(arrowPrefab, S, Quaternion.identity);
        var arrow  = go.GetComponent<ArrowController>();
        var projRb = go.GetComponent<Rigidbody2D>();
        if (projRb) projRb.isKinematic = true;
        if (arrow)  arrow.Arm(collisionDelay);

        // 화살에 트윈 핸들 전달(충돌 시 Kill 위해)
        Tweener tween = FireBezierOpenEnded(S, C, E, duration01, arrow, projRb, go);

        if (arrow != null) arrow.BindTween(tween); // ArrowController 내부에서 충돌 시 tween.Kill();
    }

    // 베지어 B(t)로 t=0→1은 도착점까지, 이후 t>1도 같은 곡선을 계속 따라감
    private Tweener FireBezierOpenEnded(
        Vector2 S, Vector2 C, Vector2 E,
        float duration01,
        ArrowController arrow, Rigidbody2D projRb, GameObject tweenTargetGO)
    {
        float t = 0f;

        // t를 0→tMaxBeyond까지 올린다. t>1에서도 같은 곡선이 자연스럽게 연장됨.
        float totalDuration = duration01 * Mathf.Max(1f, tMaxBeyond); // 간단 근사(속도 일정 아님)
        var tw = DOTween.To(() => t, v =>
            {
                t = v;
                if (projRb == null || arrow == null) return;

                // 2차 베지어 위치 B(t)
                Vector2 pos = Bezier2(S, C, E, t);
                Vector2 prev = projRb.position;
                projRb.MovePosition(pos);

                // 진행 방향 회전: B'(t)
                Vector2 dB = Bezier2Derivative(S, C, E, t);
                Vector2 dir = dB.sqrMagnitude > 1e-8f ? dB.normalized : (pos - prev).normalized;
                if (dir.sqrMagnitude > 1e-8f)
                    arrow.transform.right = dir;

            }, tMaxBeyond, totalDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Fixed)
            .SetTarget(tweenTargetGO)
            .OnComplete(() =>
            {
                // 충돌 없이 끝까지 갔을 때(아주 드묾) 마무리
                if (arrow != null) arrow.StickAndFade();
            });

        return tw;
    }

    // 2차 베지어
    private static Vector2 Bezier2(Vector2 S, Vector2 C, Vector2 E, float t)
    {
        float u = 1f - t;
        return (u*u)*S + 2f*u*t*C + (t*t)*E;
    }

    // 2차 베지어 미분(진행 방향)
    private static Vector2 Bezier2Derivative(Vector2 S, Vector2 C, Vector2 E, float t)
    {
        // B'(t) = 2(1-t)(C-S) + 2t(E-C)
        return 2f*(1f - t)*(C - S) + 2f*t*(E - C);
    }
}
