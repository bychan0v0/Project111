using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullAnchor : MonoBehaviour
{
    [Header("Pull Settings")]
    [SerializeField] private float pullStrength = 40f;   // 당기는 힘(질량/프레임에 맞춰 조정)
    [SerializeField] private float damping = 8f;         // 속도 감쇠(브레이크 비슷)
    
    [Header("Activation")]
    [SerializeField] private float pullRadius     = 3.0f; // 이 거리 안에 있을 때만 힘 적용
    [SerializeField] private float maxDuration    = 1.2f; // 앵커 유지 시간(항상 유지)
    
    [Header("Root Zone")]
    [SerializeField] private float rootChargeTime = 1.0f;    // 접촉 유지해야 루트 발동
    [SerializeField] private float rootDuration   = 2.0f;    // 루트 지속시간
    
    private Transform target;
    private Rigidbody2D targetRb;
    private CircleCollider2D rootCollider;
    
    private float contactTimer = 0f;
    private bool targetInRootZone = false;
    
    public void Init(Transform target, Rigidbody2D targetRb)
    {
        this.target = target;
        this.targetRb = targetRb;
        StartCoroutine(PullRoutine());
    }

    private IEnumerator PullRoutine()
    {
        float t = 0f;

        // (선택) 플레이어 컨트롤 잠시 비활성화가 필요하면 여기서 off → 끝나면 on
        // ex) playerController.SetInputEnabled(false);

        var originalConstraints = targetRb.constraints;
        targetRb.constraints = RigidbodyConstraints2D.FreezeRotation;

        while (t < maxDuration)
        {
            t += Time.fixedDeltaTime;

            Vector2 toAnchor = (Vector2)transform.position - targetRb.position;
            float dist = toAnchor.magnitude;

            // 범위 안이고, 스냅 거리보다 멀 때만 힘 적용
            if (dist <= pullRadius)
            {
                Vector2 dir = toAnchor / Mathf.Max(dist, 0.0001f);
                float vDot  = Vector2.Dot(targetRb.velocity, dir); // 앵커 쪽(+) / 반대(-)

                // 도망( vDot < 0 )일 때만 감쇠, 앵커 쪽으로 가면 감쇠 0
                Vector2 force = dir * (pullStrength - Mathf.Min(0f, vDot) * damping);
                targetRb.AddForce(force, ForceMode2D.Force);
            }

            if (targetInRootZone)
            {
                contactTimer += Time.fixedDeltaTime;

                // 충분히 오래 접촉했다 → 루트 발동
                if (contactTimer >= rootChargeTime)
                {
                    var player = target.GetComponent<PlayerController>();
                    StartCoroutine(RootRoutine(player));
                    
                    // 한번 발동 후, 다시 누적하려면 타이머 초기화(원한다면 그대로 유지도 가능)
                    contactTimer = 0f;
                    targetInRootZone = false; // 재발동은 존을 다시 벗어나고 들어오면 가능
                }
            }
            
            yield return new WaitForFixedUpdate();
        }

        // 종료 처리
        targetRb.constraints = originalConstraints;
        // playerController.SetInputEnabled(true);

        // VFX 잔상 등 남긴다면 여기
        Destroy(gameObject);
    }
    
    private IEnumerator RootRoutine(PlayerController player)
    {
        player.StartRoot();

        float t = 0f;
        while (t < rootDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        player.EndRoot();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        targetInRootZone = true;
        // Enter 시점에서 즉시 0으로 세팅
        contactTimer = 0f;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Stay는 AnchorRoutine에서 contactTimer 누적
        targetInRootZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        targetInRootZone = false;
        contactTimer = 0f;
    }
}
