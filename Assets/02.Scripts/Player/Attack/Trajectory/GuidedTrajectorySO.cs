using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Trajectory/Guided")]
public class GuidedTrajectorySO : TrajectorySO
{
    [Header("Speed Settings")]
    [SerializeField] private float speed = 14f;

    private Vector2 dir;

    public override void Init(Transform proj, Rigidbody2D rb, Vector2 start, Vector2 aimPoint, Transform aimTarget)
    {
        base.Init(proj, rb, start, aimPoint, aimTarget);
        dir = (aimPoint - start).sqrMagnitude > 1e-8f
            ? (aimPoint - start).normalized
            : Vector2.right;
        proj.right = dir;
    }

    public override bool Step(Transform proj, Rigidbody2D rb, float dt)
    {
        rb.MovePosition(rb.position + dir * (speed * dt));
        if (rotateToVelocity)
        {
            proj.right = dir;
        }
        return TickLife(dt);
    }
}
