using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 6f;
    
    private bool isMoving = false;
    
    private IMoveInput input;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<IMoveInput>();
    }

    private void FixedUpdate()
    {
        float x = input.GetMoveX();
        rb.velocity = new Vector2(x * moveSpeed, 0f);
    }
}
