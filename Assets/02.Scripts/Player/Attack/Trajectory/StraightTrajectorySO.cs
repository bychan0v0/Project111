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

    public override void Init(Transform proj, Rigidbody2D rb, Vector2 start, Vector2 aimPoint, Transform aimTarget)
    {
        base.Init(proj, rb, start, aimPoint, aimTarget);

        startY = start.y;

        // ��/�� ���� ���� ����
        float dx = aimPoint.x - start.x;
        dir = new Vector2(Mathf.Sign(dx), 0f);

        if (rotateToVelocity)
        {
            proj.right = dir;
        }

        // ���� y�� ��� ����
        if (rb)
        {
            rb.MovePosition(new Vector2(start.x, startY));
        }
        else
        {
            proj.position = new Vector3(start.x, startY, proj.position.z);
        }
    }

    public override bool Step(Transform proj, Rigidbody2D rb, float dt)
    {
        // x�θ� ����, y�� �߻� �������� ����
        Vector2 pos = rb.position;
        pos.x += speed * dt * dir.x;
        pos.y  = startY;
        rb.MovePosition(pos);

        if (rotateToVelocity)
        {
            proj.right = dir;
        }

        return TickLife(dt);
    }
}
