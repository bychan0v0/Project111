using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Trajectory/Down")]
public class DownTrajectorySO : TrajectorySO
{
    [Header("Speed Settings")]
    [SerializeField] private float speed = 14f;

    private float startX;
    private Vector2 dir;

    public override void Init(Rigidbody2D rb, Transform shooter, Transform target)
    {
        base.Init(rb, shooter, target);

        startX = shooter.position.x;
        dir = Vector2.down;

        if (rotateToVelocity)
        {
            shooter.right = dir;
        }
        
        rb.MovePosition(new Vector2(startX, shooter.position.y));
    }

    public override bool Step(Rigidbody2D rb, Transform shooter, float dt)
    {
        Vector2 pos = rb.position;
        pos.y += -speed * dt;
        pos.x  = startX;
        rb.MovePosition(pos);

        if (rotateToVelocity)
        {
            shooter.right = dir;
        }

        return false;
    }
}
