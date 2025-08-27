using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour, IMoveInput
{
    [SerializeField] private float leftLimit = 2f;
    [SerializeField] private float rightLimit = 8.5f;
    [SerializeField] private int dir = 0;

    public float GetMoveX()
    {
        if (transform.position.x >= rightLimit) dir = -1;
        if (transform.position.x <= leftLimit) dir = 1;
        return dir;
    }
}
