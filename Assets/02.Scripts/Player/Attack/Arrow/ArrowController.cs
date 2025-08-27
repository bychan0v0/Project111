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

    private bool running;   // ���� ���� ��
    private bool ended;   // ����(�� �̻� ������Ʈ X)

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

    // �߻� ���� ��� �浹 ����(���� �������� ��� �´� �� ����)
    public void BeginCollisionDelay()
    {
        col.enabled = false;
        DOVirtual.DelayedCall(collisionDelay, () =>
        {
            if (!ended) col.enabled = true;
        }).SetUpdate(true);
    }

    // �������/��ų���� ȣ��: ���� SO ���� + ���� ����
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

        // ���� 1������ ����. true�� �ڿ� ����(������ �浹���� ���� ����)
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
        
        // �̵� ��� ����
        running = false;

        if (isPlayer)
        {
            EndNowImmediate();
            return;
        }

        StickAndFade();   // ��� �� �ڸ��� ���� �� ���̵�
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
