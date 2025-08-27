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
    [SerializeField] private float projectileSpeed = 12f;   // m/s (t=0��1 ���� �ҿ�ð��� ���)
    [SerializeField] private float collisionDelay = 0.1f;   // �߻� ���� �浹 ���� �ð�(�ɼ�)

    [Header("Parabola (Bezier)")]
    [SerializeField] private float arcMin = 0.8f;
    [SerializeField] private float arcMax = 3.5f;
    [SerializeField] private float arcAtMaxRange = 12f;
    [SerializeField] private AnimationCurve arcCurve = AnimationCurve.Linear(0,0,1,1);

    // t>1�� �󸶳� �� ����(����). �浹�� ���� �� ���� ������.
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
        // --- ������ ---
        Vector2 S = muzzle.position;
        Vector2 E = enemy.position;

        // --- ȣ ���� ���(������ �Ÿ� ����) ---
        float distToTarget = Vector2.Distance(S, E);
        float tNorm = Mathf.Clamp01(distToTarget / Mathf.Max(0.001f, arcAtMaxRange));
        float arcH  = Mathf.Lerp(arcMin, arcMax, arcCurve.Evaluate(tNorm));

        // --- ��Ʈ�� ����Ʈ C ��� ---
        Vector2 v = E - S;
        Vector2 n = Vector2.up;   // ����Ʈ
        Vector2 M = (S + E) * 0.5f;
        Vector2 C = M + 2f * arcH * n;   // C = M + 2*d

        // --- t=0��1 ���� �ҿ�ð�(�뷫 chord ���̷� �ٻ�) ---
        float duration01 = Mathf.Max(0.05f, distToTarget / Mathf.Max(0.001f, projectileSpeed));

        // --- ����ü ����/���� ---
        var go     = Instantiate(arrowPrefab, S, Quaternion.identity);
        var arrow  = go.GetComponent<ArrowController>();
        var projRb = go.GetComponent<Rigidbody2D>();
        if (projRb) projRb.isKinematic = true;
        if (arrow)  arrow.Arm(collisionDelay);

        // ȭ�쿡 Ʈ�� �ڵ� ����(�浹 �� Kill ����)
        Tweener tween = FireBezierOpenEnded(S, C, E, duration01, arrow, projRb, go);

        if (arrow != null) arrow.BindTween(tween); // ArrowController ���ο��� �浹 �� tween.Kill();
    }

    // ������ B(t)�� t=0��1�� ����������, ���� t>1�� ���� ��� ��� ����
    private Tweener FireBezierOpenEnded(
        Vector2 S, Vector2 C, Vector2 E,
        float duration01,
        ArrowController arrow, Rigidbody2D projRb, GameObject tweenTargetGO)
    {
        float t = 0f;

        // t�� 0��tMaxBeyond���� �ø���. t>1������ ���� ��� �ڿ������� �����.
        float totalDuration = duration01 * Mathf.Max(1f, tMaxBeyond); // ���� �ٻ�(�ӵ� ���� �ƴ�)
        var tw = DOTween.To(() => t, v =>
            {
                t = v;
                if (projRb == null || arrow == null) return;

                // 2�� ������ ��ġ B(t)
                Vector2 pos = Bezier2(S, C, E, t);
                Vector2 prev = projRb.position;
                projRb.MovePosition(pos);

                // ���� ���� ȸ��: B'(t)
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
                // �浹 ���� ������ ���� ��(���� �干) ������
                if (arrow != null) arrow.StickAndFade();
            });

        return tw;
    }

    // 2�� ������
    private static Vector2 Bezier2(Vector2 S, Vector2 C, Vector2 E, float t)
    {
        float u = 1f - t;
        return (u*u)*S + 2f*u*t*C + (t*t)*E;
    }

    // 2�� ������ �̺�(���� ����)
    private static Vector2 Bezier2Derivative(Vector2 S, Vector2 C, Vector2 E, float t)
    {
        // B'(t) = 2(1-t)(C-S) + 2t(E-C)
        return 2f*(1f - t)*(C - S) + 2f*t*(E - C);
    }
}
