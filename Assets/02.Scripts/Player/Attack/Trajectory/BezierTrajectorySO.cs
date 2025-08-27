using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Trajectory/Bezier")]
public class BezierTrajectorySO : TrajectorySO
{
    [Header("Speed Settings")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float tMaxBeyond = 8f;    // t>1 ¿¬Àå
    
    [Header("Parabola Settings")]
    [SerializeField] private float arcMin = 0.8f;
    [SerializeField] private float arcMax = 3.5f;
    [SerializeField] private float arcAtMaxRange = 12f;
    [SerializeField] private AnimationCurve arcCurve = AnimationCurve.Linear(0,0,1,1);

    private Vector2 S, C, E;
    private float t, totalDuration;

    public override void Init(Rigidbody2D rb, Transform shooter, Transform target)
    {
        base.Init(rb, shooter, target);
        S = shooter.position; E = target.position;
        float dist = Vector2.Distance(S, E);
        float tn = Mathf.Clamp01(dist / Mathf.Max(0.001f, arcAtMaxRange));
        float arcH = Mathf.Lerp(arcMin, arcMax, arcCurve.Evaluate(tn));

        Vector2 n = Vector2.up;
        Vector2 M = (S + E) * 0.5f;
        C = M + 2f * arcH * n;

        float duration01 = Mathf.Max(0.05f, dist / Mathf.Max(0.001f, speed));
        totalDuration = duration01 * Mathf.Max(1f, tMaxBeyond);
        t = 0f;
    }

    public override bool Step(Rigidbody2D rb, Transform shooter, float dt)
    {
        t += dt / totalDuration * Mathf.Max(1f, tMaxBeyond);
        Vector2 pos = Bezier2(S, C, E, t);
        Vector2 prev = rb.position;
        rb.MovePosition(pos);

        if (rotateToVelocity)
        {
            Vector2 dB = Bezier2Deriv(S, C, E, t);
            Vector2 dir = dB.sqrMagnitude > 1e-8f ? dB.normalized : (pos - prev).normalized;
            if (dir.sqrMagnitude > 1e-8f) shooter.right = dir;
        }

        return t >= tMaxBeyond || TickLife(dt);
    }

    private Vector2 Bezier2(Vector2 S, Vector2 C, Vector2 E, float t)
    {
        float u = 1f - t;
        return u*u*S + 2f*u*t*C + t*t*E;
    }
    private Vector2 Bezier2Deriv(Vector2 S, Vector2 C, Vector2 E, float t)
    {
        return 2f*(1f - t)*(C - S) + 2f*t*(E - C);
    }
}
