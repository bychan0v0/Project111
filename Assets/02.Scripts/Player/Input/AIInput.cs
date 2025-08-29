using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour, IMoveInput
{
    [SerializeField] private float leftLimit = 2f;
    [SerializeField] private float rightLimit = 8.5f;
    [SerializeField] private int dir = 0;

    private bool overrideActive;
    private float overrideUntil;

    private bool noGoActive;
    private Bounds noGoBounds;
    private float noGoPad;
    private float noGoUntil;

    public void SetDir(int d, float duration = 0f)
    {
        overrideActive = true;
        overrideUntil  = (duration > 0f) ? Time.time + duration : 0f;
        dir = Mathf.Clamp(d, -1, 1);
    }

    public void SetNoGoZone(Bounds zoneWorldBounds, float pad = 0.75f, float duration = 2.0f)
    {
        noGoActive = true;
        noGoBounds = zoneWorldBounds;
        noGoPad    = Mathf.Max(0f, pad);
        noGoUntil  = (duration > 0f) ? Time.time + duration : 0f;
    }

    public float GetMoveX()
    {
        float x = transform.position.x;

        if (x >= rightLimit) return -1;
        if (x <= leftLimit)  return  1;

        if (noGoActive)
        {
            if (noGoUntil > 0f && Time.time >= noGoUntil)
            {
                noGoActive = false;
            }
            else
            {
                float left  = noGoBounds.min.x - noGoPad;
                float right = noGoBounds.max.x + noGoPad;

                if (x > left && x < right)
                {
                    float distToLeft  = x - left;
                    float distToRight = right - x;
                    int away = (distToLeft < distToRight) ? -1 : +1;
                    return away;
                }
            }
        }

        if (overrideActive)
        {
            if (overrideUntil > 0f && Time.time >= overrideUntil)
                overrideActive = false;
            else
                return dir;
        }

        return dir;
    }
}
