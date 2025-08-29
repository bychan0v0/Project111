using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkillAction_PullAnchor : SkillActionBase
{
    [Header("Anchor")]
    [SerializeField] private GameObject pullAnchorPrefab;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundRayDistance = 5f;
    
    public override void Execute(in SkillContext ctx)
    {
        ctx.skillManager?.EnterCast();
        
        FireOneArrow(ctx, arrowPrefab);
        
        ctx.skillManager?.ExitCast();
    }
    
    protected override void OnArrowHit(in SkillContext ctx, Vector2 hitPoint, Collider2D hitCol)
    {
        Vector2 anchorPos = hitPoint;

        bool hitPlayer = (hitCol != null) && ((1 << hitCol.gameObject.layer) == (1 << ctx.target.gameObject.layer));
        if (hitPlayer)
        {
            var ray = Physics2D.Raycast(hitPoint + Vector2.up * 0.55f, Vector2.down, groundRayDistance, groundMask);
            if (ray.collider != null)
            {
                anchorPos = ray.point + Vector2.up * 0.5f;
            }
        }

        var anchor = Instantiate(pullAnchorPrefab, anchorPos + Vector2.up * 0.5f, Quaternion.identity);
        var pull = anchor.GetComponent<PullAnchor>();
        if (pull != null)
        {
            pull.Init(ctx.target.transform, ctx.target.GetComponent<Rigidbody2D>());
        }
    }
}
