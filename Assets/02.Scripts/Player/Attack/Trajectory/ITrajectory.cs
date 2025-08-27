using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrajectory
{
    void Init(Transform proj, Rigidbody2D rb, Vector2 start, Vector2 aimPoint, Transform aimTarget);
    bool Step(Transform proj, Rigidbody2D rb, float dt);
}

