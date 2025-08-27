using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrajectorySO : ScriptableObject, ITrajectory
{
    [Header("Common Options")]
    [SerializeField] protected bool rotateToVelocity = true;
    [SerializeField] protected float maxLifeTime = 6f;
    protected float life;

    public virtual void Init(Rigidbody2D rb, Transform shooter, Transform target)
    {
        life = 0f;
    }

    public abstract bool Step(Rigidbody2D rb, Transform shooter, float dt);

    protected bool TickLife(float dt)
    {
        life += dt;
        return life >= maxLifeTime;
    }
}
