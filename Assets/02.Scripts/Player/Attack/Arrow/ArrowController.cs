using System;
using DG.Tweening;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private TrajectorySO trajectorySO;

    private Rigidbody2D rb;
    private bool running;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // �������/��ų���� ȣ��: ���� SO�� ����/��ǥ ����
    public void SetupTrajectory(TrajectorySO so, Vector3 startPos, Vector3 aimPoint, Transform aimTarget = null)
    {
        trajectorySO = so;
        transform.position = startPos;
        trajectorySO?.Init(transform, rb, startPos, aimPoint, aimTarget);

        running = trajectorySO != null;
    }

    public void Stop() => running = false;

    private void FixedUpdate()
    {
        if (!running || trajectorySO == null) return;

        bool naturalEnd = trajectorySO.Step(transform, rb, Time.fixedDeltaTime);
        if (naturalEnd)
        {
            running = false;
            // �ڿ� ����(�浹 ����) �� ����Ʈ�� ����
            GetComponent<ArrowCollision>()?.OnTrajectoryFinished();
        }
    }
}
