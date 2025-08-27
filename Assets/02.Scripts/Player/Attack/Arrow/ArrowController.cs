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

    // 오토어택/스킬에서 호출: 궤적 SO와 시작/목표 세팅
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
            // 자연 종료(충돌 없이) 시 임팩트에 전달
            GetComponent<ArrowCollision>()?.OnTrajectoryFinished();
        }
    }
}
