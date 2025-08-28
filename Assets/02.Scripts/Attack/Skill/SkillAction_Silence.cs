using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkillAction_Silence : SkillActionBase
{
    private static readonly int SILENCE = Animator.StringToHash("Silence");

    [Header("Silence Duration")]
    [SerializeField] private float duration;
    
    private IEnumerator AnimationDelay(SkillContext ctx)
    {
        ctx.skillManager?.EnterCast();
        
        animator.ResetTrigger(SILENCE);
        animator.SetTrigger(SILENCE);
        
        while (true)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            if (st.IsName("Silence") && st.normalizedTime >= 0.8f)
            {
                FireOneArrow(ctx);
                break;
            }
            
            yield return null;
        }
        
        ctx.skillManager?.ExitCast();
    }
    
    private IEnumerator SilenceRoutine(SkillManager skill)
    {
        skill.SetSilence();
        yield return new WaitForSeconds(duration);
        skill.ResetSilence();
    }
    
    public override void Execute(in SkillContext ctx)
    {
        StartCoroutine(AnimationDelay(ctx));
    }
    
    protected override void OnArrowHit(in SkillContext ctx, Vector2 hitPoint, Collider2D hitCol)
    {
        bool hitPlayer = (hitCol != null) && ((1 << hitCol.gameObject.layer) == (1 << ctx.target.gameObject.layer));
        if (hitPlayer)
        {
            var skill = ctx.target.GetComponentInChildren<SkillManager>();
            StartCoroutine(SilenceRoutine(skill));
        }
    }
}
