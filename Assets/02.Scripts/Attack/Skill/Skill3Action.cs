using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Skill3Action : MonoBehaviour, ISkillBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private TrajectorySO trajectorySO;
    
    [Header("Silence Duration")]
    [SerializeField] private float duration;
    
    private bool busy;
    
    public void Execute(in SkillContext ctx)
    {
        FireOneArrow(ctx);
    }

    private void FireOneArrow(in SkillContext ctx)
    {
        // ȭ�� ���� + ���� ����(ȭ�츶�� SO �ν��Ͻ� ����)
        var go = Instantiate(arrowPrefab, ctx.muzzle.position, Quaternion.identity);
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
    
    private void OnArrowHit(in SkillContext ctx, Vector2 hitPoint, Collider2D hitCol)
    {
        bool hitPlayer = (hitCol != null) && ((1 << hitCol.gameObject.layer) == (1 << ctx.target.gameObject.layer));
        if (hitPlayer)
        {
            var skill = ctx.target.GetComponentInChildren<SkillManager>();
            StartCoroutine(SilenceRoutine(skill));
        }
    }

    private IEnumerator SilenceRoutine(SkillManager skill)
    {
        skill.SetSilence();
        yield return new WaitForSeconds(duration);
        skill.ResetSilence();
    }
}
