using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAction_LuckyShot : SkillActionBase
{
    private static readonly int SILENCE = Animator.StringToHash("Silence");
    
    [Header("Custom Arrow")]
    [SerializeField] private GameObject luckyArrowPrefab;

    [Header("Arrow Settings")]
    [SerializeField] private int arrowCount = 5;
    [SerializeField] private float burstInterval = 0.05f;
    
    [Header("Lucky Params")]
    [SerializeField, Range(0f,1f)] private float bonusChance = 0.35f; // 보너스 피해 확률
    [SerializeField] private int bonusDamage = 10;

    [SerializeField, Range(0f,1f)] private float backfireChance = 0.07f; // 역효과 확률
    [SerializeField] private int backfireDamage = 3;

    [SerializeField] private Vector3 uiOffset = new Vector3(0, 1.6f, 0);

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
                for (int i = 0; i < arrowCount; i++)
                {
                    FireOneArrow(ctx, luckyArrowPrefab);
                    if (i < arrowCount - 1 && burstInterval > 0f)
                        yield return new WaitForSeconds(burstInterval);
                }
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

    protected override void OnArrowHit(in SkillContext ctx, Vector2 p, Collider2D col)
    {
        if (!IsHitTarget(ctx, col)) return;

        // 1) 보너스 피해 굴림
        if (Random.value < bonusChance)
        {
            if (TryGetDamageable(col, out var dmg) && dmg.IsAlive)
            {
                dmg.TakeDamage(bonusDamage);
                HitUIRoot.Instance?.ShowDamage(bonusDamage, p);
                HitUIRoot.Instance?.ShowStatusOver(col.transform.root, "CRIT!", 0.6f, uiOffset);
            }
            return; // 보너스 났으면 역효과는 생략(원하면 둘 다 굴리게 바꿔도 됨)
        }

        // 2) 아주 낮은 확률로 역효과(시전자 자해)
        if (Random.value < backfireChance)
        {
            var selfHp = ctx.caster.GetComponentInChildren<PlayerHp>();
            if (selfHp && selfHp.IsAlive)
            {
                selfHp.TakeDamage(backfireDamage);
                HitUIRoot.Instance?.ShowDamage(backfireDamage, ctx.caster.position + Vector3.up * 1.2f);
                HitUIRoot.Instance?.ShowStatusOver(ctx.caster, "Backfire!", 0.6f, uiOffset);
            }
        }
    }

    bool TryGetDamageable(Collider2D col, out IDamageable d)
    {
        d = col.GetComponentInParent<IDamageable>();
        return d != null;
    }
}
