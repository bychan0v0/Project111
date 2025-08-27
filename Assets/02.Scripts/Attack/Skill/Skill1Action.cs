using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Skill1Action : MonoBehaviour, ISkillBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private TrajectorySO trajectorySO;

    [Header("Anchor")]
    [SerializeField] private GameObject pullAnchorPrefab; // <= 새 프리팹
    [SerializeField] private LayerMask groundMask;         // 바닥 레이어
    [SerializeField] private float groundRayDistance = 5f; // 플레이어를 친 경우, 아래 탐색 거리
    
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
        Vector2 anchorPos = hitPoint;

        // 플레이어를 맞췄다면, 그 지점 바로 아래 땅을 찾아서 고정
        bool hitPlayer = (hitCol != null) && ((1 << hitCol.gameObject.layer) == (1 << ctx.target.gameObject.layer));
        if (hitPlayer)
        {
            var ray = Physics2D.Raycast(hitPoint + Vector2.up * 0.05f, Vector2.down, groundRayDistance, groundMask);
            if (ray.collider != null)
            {
                anchorPos = ray.point + Vector2.up * 0.5f;
            }
            // 못찾았으면 그냥 hitPoint 사용(플로팅 앵커)
        }

        // 앵커 소환
        var anchor = Instantiate(pullAnchorPrefab, anchorPos, Quaternion.identity);
        var pull = anchor.GetComponent<PullAnchor>();
        if (pull != null)
        {
            pull.Init(ctx.target.transform, ctx.target.GetComponent<Rigidbody2D>());
        }

        // (선택) 바닥에 스티커처럼 붙도록 Z정렬/파티클/사운드 등 추가
    }
}
