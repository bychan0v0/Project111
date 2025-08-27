using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SkillManager skillManager;

    [Header("AutoAttack Settings")]
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
            skillManager.UseSkill("AutoAttack");
            timer = 0f;
        }
    }
}