using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrajectorySO : ScriptableObject, ITrajectory
{
    [Header("Common Options")]
    [SerializeField] protected bool rotateToVelocity = true;
    [SerializeField] protected float maxLifeTime = 6f;
    protected float life;

    public virtual void Init(Transform proj, Rigidbody2D rb, Vector2 start, Vector2 aimPoint, Transform aimTarget)
    {
        life = 0f;
    }

    public abstract bool Step(Transform proj, Rigidbody2D rb, float dt);

    protected bool TickLife(float dt)
    {
        life += dt;
        return life >= maxLifeTime;
    }
}
