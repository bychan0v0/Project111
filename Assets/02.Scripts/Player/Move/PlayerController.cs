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
    private SkillManager skillManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<IMoveInput>();
        skillManager = GetComponentInChildren<SkillManager>();
    }

    private void FixedUpdate()
    {
        // 스킬 캐스팅 중이면 이동 금지
        if (skillManager && skillManager.IsCasting)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        
        float x = input.GetMoveX();
        rb.velocity = new Vector2(x * moveSpeed, 0f);
    }
}
