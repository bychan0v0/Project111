using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkillAction_Poison : SkillActionBase
{
    private static readonly int SILENCE = Animator.StringToHash("Silence");
    
    [Header("Custom Arrow")]
    [SerializeField] private GameObject poisonArrowPrefab;
    
    [Header("Poison")]
    [SerializeField, Min(1)] int damagePerTick = 2;
    [SerializeField, Min(0.05f)] float tickInterval = 0.5f;
    [SerializeField, Min(0.1f)] float duration = 4f;
    [SerializeField] string statusText = "Poison";
    [SerializeField] Vector3 uiWorldOffset = new Vector3(0f, 1.6f, 0f);

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
                FireOneArrow(ctx, poisonArrowPrefab);
                break;
            }
            
            yield return null;
        }
        
        ctx.skillManager?.ExitCast();
    }
    
    public override void Execute(in SkillContext ctx)
    {
        StartCoroutine(AnimationDelay(ctx));
    }

    protected override void OnArrowHit(in SkillContext ctx, Vector2 hitPoint, Collider2D hitCol)
    {
        if (!IsHitTarget(ctx, hitCol)) return;

        var targetRoot = ctx.target ? ctx.target : hitCol.transform;
        var ticker = targetRoot.GetComponentInChildren<PoisonTicker>();
        if (!ticker) ticker = targetRoot.gameObject.AddComponent<PoisonTicker>();

        ticker.Apply(damagePerTick, tickInterval, duration, statusText, uiWorldOffset);
    }
}

// 재사용 가능한 도트 컴포넌트
public class PoisonTicker : MonoBehaviour
{
    Coroutine co;

    public void Apply(int dmg, float tick, float dur, string label, Vector3 uiOffset)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Run(dmg, tick, dur, label, uiOffset));
    }

    IEnumerator Run(int dmg, float tick, float dur, string label, Vector3 uiOffset)
    {
        var hp = GetComponentInChildren<PlayerHp>();
        if (!hp) yield break;

        HitUIRoot.Instance?.ShowStatusOver(transform, label, dur, uiOffset);

        float t = 0f, acc = 0f;
        while (t < dur && hp && hp.IsAlive)
        {
            acc += Time.deltaTime;
            if (acc >= tick)
            {
                acc -= tick;
                hp.TakeDamage(dmg);
                HitUIRoot.Instance?.ShowDamage(dmg, hp.transform.position + Vector3.up * 1.2f);
            }
            t += Time.deltaTime;
            yield return null;
        }
        co = null;
    }
}
