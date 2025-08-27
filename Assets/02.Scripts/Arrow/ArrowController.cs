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

    private Tweener boundTween;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sprite;
    private bool ended;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // 충돌/트리거 전용 물리셋업
        rb.gravityScale = 0f;
        rb.isKinematic  = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        col.isTrigger = true;

        sprite = GetComponent<SpriteRenderer>();
    }

    // 발사 직후 충돌 판정 지연
    public void Arm(float collisionDelaySec)
    {
        col.enabled = false;
        DOVirtual.DelayedCall(collisionDelaySec, () =>
        {
            col.enabled = true;
        }).SetUpdate(true);
    }

    public void BindTween(Tweener tw) => boundTween = tw;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ended) return;

        int otherLayer = other.gameObject.layer;
        bool isGround = (groundMask.value & (1 << otherLayer)) != 0;
        bool isPlayer = (playerMask.value & (1 << otherLayer)) != 0;
        
        if (isPlayer)
        {
            EndNowImmediate();
            return;
        }

        if (isGround)
        {
            StickAndFade();
            return;
        }
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

        if (boundTween != null && boundTween.IsActive())
        {
            boundTween.Kill();
        }
        
        rb.velocity = Vector2.zero;
        col.enabled = false;

        if (sprite) sprite.DOFade(0f, fadeDuration).SetUpdate(true);
        DOVirtual.DelayedCall(fadeDuration, () => Destroy(gameObject)).SetUpdate(true);
    }
}
