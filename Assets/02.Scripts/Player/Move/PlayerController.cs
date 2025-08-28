using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 6f;
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGround = false;
    
    [Header("Control Lock")]
    [SerializeField] private bool blockInput = false;     // 입력 막기 토글
    [SerializeField] private Behaviour[] disableOnLock;   // 루트 동안 꺼둘 컴포넌트(SkillManager, AutoAttack 등 등록)
    
    private Collider2D col; 
    private Rigidbody2D rb;
    private IMoveInput input;
    private SkillManager skillManager;
    
    public bool IsGround => isGround;
    
    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<IMoveInput>();
        skillManager = GetComponentInChildren<SkillManager>();
    }

    private void FixedUpdate()
    {
        UpdateGrounded();
        
        if (!isGround || skillManager.IsCasting)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }
        
        float x = input.GetMoveX();
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
    }
    
    private void UpdateGrounded()
    {
        if (col.IsTouchingLayers(groundMask))
        {
            isGround = true;
            return;
        }

        isGround = false;
    }

    public void StartRoot()
    {
        blockInput = true;
        
        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        // 등록된 컴포넌트 비활성화
        if (disableOnLock != null)
        {
            foreach (var b in disableOnLock)
            {
                b.enabled = false;
            }
        }
    }
    
    public void EndRoot()
    {
        blockInput = false;

        // 컴포넌트 다시 켜기
        if (disableOnLock != null)
        {
            foreach (var b in disableOnLock)
            {
                b.enabled = true;
            }
        }
    }
}
