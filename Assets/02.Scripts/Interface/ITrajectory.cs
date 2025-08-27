using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrajectory
{
    void Init(Rigidbody2D rb, Transform shooter, Transform target);
    bool Step(Rigidbody2D rb, Transform start, float dt);
}

