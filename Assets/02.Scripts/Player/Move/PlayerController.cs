using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static readonly int IS_RUN = Animator.StringToHash("isRun");

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask;
    
    private bool isGround = false;
    private bool isMoving = false;
    private bool isRoot = false;

    private Collider2D col;
    private Rigidbody2D rb;
    private IMoveInput input;
    private SkillManager skillManager;
    private Animator animator;

    public bool IsGround => isGround;
    public bool IsMoving => isMoving;
    public bool IsRoot => isRoot;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<IMoveInput>();
        skillManager = GetComponentInChildren<SkillManager>();

        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        UpdateGrounded();

        if (!isGround || skillManager.IsCasting)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            SetRun(false);
            return;
        }

        float x = input.GetMoveX();
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        UpdateMoving(x);
        SetRun(Mathf.Abs(x) > 0.01f);
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

    private void UpdateMoving(float x)
    {
        if (Mathf.Abs(x) > 0.01f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    public void StartRoot(float rootDuration)
    {
        StartCoroutine(RootRoutine(rootDuration));
    }

    private IEnumerator RootRoutine(float rootDuration)
    {
        isRoot = true;
        
        float t = 0f;
        while (t < rootDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }
        
        isRoot = false;
    }
    
    private void SetRun(bool value)
    {
        animator.SetBool(IS_RUN, value);
    }
}
