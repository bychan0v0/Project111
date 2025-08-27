using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Trajectory/Guided")]
public class GuidedTrajectorySO : TrajectorySO
{
    [Header("Speed Settings")]
    [SerializeField] private float speed = 14f;

    private Vector2 dir;

    public override void Init(Rigidbody2D rb, Transform shooter, Transform target)
    {
        base.Init(rb, shooter, target);
        dir = (target.position - shooter.position).sqrMagnitude > 1e-8f
            ? (target.position - shooter.position).normalized
            : Vector2.right;
        shooter.right = dir;
    }

    public override bool Step(Rigidbody2D rb, Transform shooter, float dt)
    {
        rb.MovePosition(rb.position + dir * (speed * dt));
        if (rotateToVelocity)
        {
            shooter.right = dir;
        }
        return TickLife(dt);
    }
}
