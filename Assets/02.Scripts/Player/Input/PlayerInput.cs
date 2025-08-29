using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour, IMoveInput
{
    private float x;

    private void Update()
    {
        x = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x = 1f;
        }
    }

    public void OnLeftDown()
    {
        x = -1f;
    }
    public void OnRightDown()
    {
        x = 1f;
    }
    public float GetMoveX()
    {
        return x;
    }
}
