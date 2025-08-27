using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingController : MonoBehaviour
{
    [Header("Facing Settings")]
    private int idleFaceDir = 1;
    private SpriteRenderer sprite;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        Vector3 selfPos = transform.position;
        idleFaceDir = 0f - selfPos.x >= 0f ? 1 : -1;
    }
    
    private void LateUpdate()
    {
        float vx = rb.velocity.x;
        int dir = Mathf.Abs(vx) > 0.001f ? (vx > 0f ? 1 : -1) : idleFaceDir;
        sprite.flipX = dir < 0;
    }
}
