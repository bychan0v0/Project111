using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SkillManager skillManager;

    [Header("AutoAttack Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private TrajectorySO trajectorySO;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform target;
    [SerializeField] private float attackInterval = 0.6f;
    [SerializeField] private float idleEps = 0.02f;

    private Rigidbody2D rb;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        skillManager = GetComponentInChildren<SkillManager>();
    }

    private void Update()
    {
        if (skillManager.IsCasting) return;
        
        timer += Time.deltaTime;
        bool isIdle = !rb || Mathf.Abs(rb.velocity.x) <= idleEps;

        if (isIdle && timer >= attackInterval)
        {
            AutoAttack();
            timer = 0f;
        }
    }

    private void AutoAttack()
    {
        // 화살 생성 및 궤적 세팅
        var go  = Instantiate(arrowPrefab, muzzle.position, Quaternion.identity);
        var proj = go.GetComponent<ArrowController>();

        var soInstance = Instantiate(trajectorySO);

        proj.BeginCollisionDelay();
        proj.SetupTrajectory(soInstance, muzzle.position, target);
    }
}