using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Skill2Action : MonoBehaviour, ISkillBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private TrajectorySO trajectorySO;

    [Header("Flip Motion")]
    [SerializeField, Min(0f)] private float jumpImpulse = 7.5f;   // 위로 튀어오를 힘
    [SerializeField, Min(0.2f)] private float totalFlipTime = 0.9f;   // 총 3바퀴 시간(빠르게)

    [Header("Volley")]
    [SerializeField, Min(1)] private int arrowCount = 3;   // 항상 3발
    [SerializeField, Min(0f)] private float burstInterval = 0.06f;   // 3발 간 간격
    [SerializeField, Min(0f)] private float burstDelay = 0.75f;   // 발사 대기 시간
    
    private bool busy;
    
    public void Execute(in SkillContext ctx)
    {
        if (busy) return;
        StartCoroutine(DoFlipAndVolley(ctx));
    }

    private IEnumerator DoFlipAndVolley(SkillContext ctx)
    {
        busy = true;
        ctx.skillManager?.EnterCast();

        // 1) 점프 임펄스
        var rb = ctx.rb;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);

        // 2) 시각 메쉬만 1080°(3회전) ? Z축 기준(2D)
        var spinTarget = ctx.caster;
        spinTarget
            .DOLocalRotate(new Vector3(0f, 0f, 1080f), totalFlipTime, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear);

        yield return new WaitForSeconds(burstDelay);
        
        // 3) 회전하는 동안 3발 간격 발사
        for (int i = 0; i < arrowCount; i++)
        {
            FireOneArrow(ctx);
            if (i < arrowCount - 1 && burstInterval > 0f)
                yield return new WaitForSeconds(burstInterval);
        }

        // 4) 회전 각 정리
        var e = spinTarget.localEulerAngles;
        spinTarget.localEulerAngles = new Vector3(e.x, e.y, Mathf.Repeat(e.z, 360f));
        
        busy = false;
        ctx.skillManager?.ExitCast();
    }

    private void FireOneArrow(in SkillContext ctx)
    {
        // 화살 생성 + 궤적 세팅(화살마다 SO 인스턴스 독립)
        var go = Instantiate(arrowPrefab, ctx.muzzle.position, Quaternion.identity);
        var proj = go.GetComponent<ArrowController>();
        var soInstance = Instantiate(trajectorySO);

        if (proj != null)
        {
            proj.BeginCollisionDelay();
            proj.SetupTrajectory(soInstance, ctx.muzzle.position, ctx.target);
        }
    }
}
