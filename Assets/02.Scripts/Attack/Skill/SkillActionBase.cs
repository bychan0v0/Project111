using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillActionBase : MonoBehaviour, ISkillBehaviour
{
    [Header("Refs")]
    [SerializeField] private TrajectorySO trajectorySO;

    protected PlayerController playerController;
    protected AttackManager attackManager;
    protected Animator animator;
    protected GameObject arrowPrefab;
    
    protected virtual void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        attackManager = GetComponentInParent<AttackManager>();
        animator = GetComponentInParent<Animator>();
        
        arrowPrefab = attackManager.GetArrowPrefab();
    }
    
    protected void FireOneArrow(in SkillContext ctx)
    {
        // 화살 생성 + 궤적 세팅(화살마다 SO 인스턴스 독립)
        var go = Instantiate(arrowPrefab, ctx.muzzle.position, ctx.muzzle.rotation);
        var proj = go.GetComponent<ArrowController>();
        var soInstance = Instantiate(trajectorySO);

        if (proj != null)
        {
            proj.BeginCollisionDelay();
            proj.SetupTrajectory(soInstance, ctx.muzzle.position, ctx.target);
            
            var context = ctx;
            proj.OnFirstHit += (hitPoint, hitCol) => OnArrowHit(context, hitPoint, hitCol);
        }
    }
    
    public abstract void Execute(in SkillContext ctx);

    protected abstract void OnArrowHit(in SkillContext ctx, Vector2 hitPoint, Collider2D hitCol);
    
}
