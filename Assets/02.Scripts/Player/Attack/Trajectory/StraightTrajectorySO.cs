using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Trajectory/Straight")]
public class StraightTrajectorySO : TrajectorySO
{
    [Header("Speed Settings")]
    [SerializeField] private float speed = 14f;

    private float startY;
    private Vector2 dir;

    public override void Init(Rigidbody2D rb, Transform shooter, Transform target)
    {
        base.Init(rb, shooter, target);

        startY = shooter.position.y;

        // ��/�� ���� ���� ����
        float dx = target.position.x - shooter.position.x;
        dir = new Vector2(Mathf.Sign(dx), 0f);

        if (rotateToVelocity)
        {
            shooter.right = dir;
        }

        // ���� y�� ��� ����
        rb.MovePosition(new Vector2(shooter.position.x, startY));
    }

    public override bool Step(Rigidbody2D rb, Transform shooter, float dt)
    {
        // x�θ� ����, y�� �߻� �������� ����
        Vector2 pos = rb.position;
        pos.x += speed * dt * dir.x;
        pos.y  = startY;
        rb.MovePosition(pos);

        if (rotateToVelocity)
        {
            shooter.right = dir;
        }

        return TickLife(dt);
    }
}
