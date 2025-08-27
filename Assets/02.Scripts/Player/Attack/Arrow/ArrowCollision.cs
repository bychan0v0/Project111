using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArrowCollision : MonoBehaviour
{
    [Header("Masks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;

    [Header("Visual")]
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("Collision Delay")]
    [SerializeField] private float collisionDelay = 0.15f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sprite;

    private bool ended;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();

        rb.isKinematic = true;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        col.isTrigger = true;
    }

    /// <summary>풀에서 꺼낼 때 상태 초기화</summary>
    public void ResetForReuse()
    {
        ended = false;
        col.enabled = false; // 곧 무적 딜레이로 켜짐
        var c = sprite.color; c.a = 1f; sprite.color = c;
    }

    /// <summary>스폰 직후 잠깐 충돌 무시</summary>
    public void BeginCollisionDelay()
    {
        col.enabled = false;
        DOVirtual.DelayedCall(collisionDelay, () =>
        {
            if (!ended)
            {
                col.enabled = true;
            }
        }).SetUpdate(true);
    }

    public void OnTrajectoryFinished()
    {
        if (ended)
        {
            return;
        }
        StickAndFade();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ended)
        {
            return;
        }

        int layer = other.gameObject.layer;

        // 화이트리스트가 아니면 무시(다른 화살/센서 등)
        if (((groundMask.value | playerMask.value) & (1 << layer)) == 0) return;

        // 이동 중지
        GetComponent<ArrowController>()?.Stop();

        bool isGround = (groundMask.value & (1 << layer)) != 0;
        bool isPlayer = (playerMask.value & (1 << layer)) != 0;

        if (isPlayer) { EndNowImmediate(); return; }
        if (isGround) { StickAndFade();  return; }

        StickAndFade();
    }

    public void EndNowImmediate()
    {
        if (ended)
        {
            return;
        }
        ended = true;
        ObjectPool.Instance.Return(gameObject); // 바로 풀 복귀
    }

    private void StickAndFade()
    {
        if (ended)
        {
            return;
        }
        ended = true;

        rb.velocity = Vector2.zero;
        col.enabled = false;

        sprite.DOFade(0f, fadeDuration).SetUpdate(true);
        DOVirtual.DelayedCall(fadeDuration, () =>
        {
            ObjectPool.Instance.Return(gameObject);
        }).SetUpdate(true);
    }
}
