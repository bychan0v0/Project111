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

    /// <summary>Ǯ���� ���� �� ���� �ʱ�ȭ</summary>
    public void ResetForReuse()
    {
        ended = false;
        col.enabled = false; // �� ���� �����̷� ����
        var c = sprite.color; c.a = 1f; sprite.color = c;
    }

    /// <summary>���� ���� ��� �浹 ����</summary>
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

        // ȭ��Ʈ����Ʈ�� �ƴϸ� ����(�ٸ� ȭ��/���� ��)
        if (((groundMask.value | playerMask.value) & (1 << layer)) == 0) return;

        // �̵� ����
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
        ObjectPool.Instance.Return(gameObject); // �ٷ� Ǯ ����
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
