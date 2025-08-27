using System;
using DG.Tweening;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.25f;
    
    private float collisionDelay = 0.5f;
    
    private TrajectorySO trajectorySO;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sprite;

    private bool running;   // 궤적 실행 중
    private bool ended;   // 종료(더 이상 업데이트 X)

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.isKinematic  = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        col.isTrigger = true;
    }

    // 발사 직후 잠깐 충돌 무시(스폰 지점에서 즉시 맞는 것 방지)
    public void BeginCollisionDelay()
    {
        col.enabled = false;
        DOVirtual.DelayedCall(collisionDelay, () =>
        {
            if (!ended) col.enabled = true;
        }).SetUpdate(true);
    }

    // 오토어택/스킬에서 호출: 궤적 SO 장착 + 실행 시작
    public void SetupTrajectory(TrajectorySO so, Vector3 startPos, Transform aimTarget)
    {
        trajectorySO = so;
        transform.position = startPos;

        rb = GetComponent<Rigidbody2D>();
        trajectorySO?.Init(rb, transform, aimTarget);

        running = trajectorySO != null;
        ended = false;
    }

    private void FixedUpdate()
    {
        if (!running || ended || trajectorySO == null) return;

        // 궤적 1프레임 진행. true면 자연 종료(보통은 충돌에서 먼저 끝남)
        bool naturalEnd = trajectorySO.Step(rb, transform, Time.fixedDeltaTime);
        if (naturalEnd)
        {
            running = false;
            StickAndFade();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ended)
        {
            return;
        }

        int otherLayer = other.gameObject.layer;
        bool isGround = (groundMask.value & (1 << otherLayer)) != 0;
        bool isPlayer = (playerMask.value & (1 << otherLayer)) != 0;

        if (!isGround && !isPlayer)
        {
            return;
        }
        
        // 이동 즉시 정지
        running = false;

        if (isPlayer)
        {
            EndNowImmediate();
            return;
        }

        StickAndFade();   // 즉시 그 자리에 멈춘 뒤 페이드
    }

    public void EndNowImmediate()
    {
        if (ended) return;
        ended = true;
        Destroy(gameObject);
    }

    public void StickAndFade()
    {
        if (ended) return;
        ended = true;

        rb.velocity = Vector2.zero;
        col.enabled = false;

        sprite.DOFade(0f, fadeDuration).SetUpdate(true);
        DOVirtual.DelayedCall(fadeDuration, () => Destroy(gameObject)).SetUpdate(true);
    }
}
