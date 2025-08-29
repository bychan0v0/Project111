using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private static readonly int FIRE = Animator.StringToHash("Fire");

    [Header("AutoAttack Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private TrajectorySO trajectorySO;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform target;
    [SerializeField] private float attackInterval = 0.6f;
    [SerializeField] private float idleEps = 0.02f;
    [SerializeField, Min(0.05f)] float attackClipLength = 0.7f;

    private PlayerController playerController;
    private SkillManager skillManager;
    private Rigidbody2D rb;
    private Animator animator;
    
    private float timer;
    private bool armed = true;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        skillManager = GetComponentInChildren<SkillManager>();
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (skillManager.IsCasting || playerController.IsMoving || !playerController.IsGround || playerController.IsRoot)
        {
            armed = false;
            return;
        }
        
        timer += Time.deltaTime;
        bool isIdle = Mathf.Abs(rb.velocity.x) <= idleEps;

        if (isIdle && timer >= attackInterval)
        {
            animator.speed = Mathf.Max(0.01f, attackClipLength / Mathf.Max(0.01f, attackInterval));
            animator.ResetTrigger(FIRE);
            animator.SetTrigger(FIRE);
            
            armed = true;
        }
        
        var st = animator.GetCurrentAnimatorStateInfo(0);
        if (armed && st.IsName("Fire") && st.normalizedTime >= 0.8f)
        {
            armed = false;
            timer = 0f;
            
            AutoAttack();
        }
    }

    public void AutoAttack()
    {
        var go  = Instantiate(arrowPrefab, muzzle.position, muzzle.rotation);
        var proj = go.GetComponent<ArrowController>();

        var soInstance = Instantiate(trajectorySO);

        proj.BeginCollisionDelay();
        proj.SetupTrajectory(soInstance, muzzle.position, target);
    }

    public GameObject GetArrowPrefab()
    {
        return arrowPrefab;
    }
}