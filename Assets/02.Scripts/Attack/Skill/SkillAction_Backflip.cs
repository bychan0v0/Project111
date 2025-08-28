using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkillAction_Backflip : SkillActionBase
{
    private static readonly int BACKFLIP = Animator.StringToHash("Backflip");

    [Header("Flip Motion")]
    [SerializeField, Min(0f)] private float jumpImpulse = 7.5f;   // ���� Ƣ����� ��
    [SerializeField, Min(0.2f)] private float totalFlipTime = 0.9f;   // �� 3���� �ð�(������)

    [Header("Volley")]
    [SerializeField, Min(1)] private int arrowCount = 3;   // �׻� 3��
    [SerializeField, Min(0f)] private float burstInterval = 0.06f;   // 3�� �� ����
    [SerializeField, Min(0f)] private float burstDelay = 0.75f;   // �߻� ��� �ð�

    private IEnumerator DoFlipAndVolley(SkillContext ctx)
    {
        ctx.skillManager?.EnterCast();

        animator.ResetTrigger(BACKFLIP);
        animator.SetTrigger(BACKFLIP);
        
        // 1) ���� ���޽�
        var rb = ctx.rb;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        
        // 2) �ð� �޽��� 1080��(3ȸ��) ? Z�� ����(2D)
        var spinTarget = ctx.caster;
        spinTarget
            .DOLocalRotate(new Vector3(0f, 0f, 1080f), totalFlipTime, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear);

        yield return new WaitForSeconds(burstDelay);
        
        // 3) ȸ���ϴ� ���� 3�� ���� �߻�
        for (int i = 0; i < arrowCount; i++)
        {
            FireOneArrow(ctx);
            if (i < arrowCount - 1 && burstInterval > 0f)
                yield return new WaitForSeconds(burstInterval);
        }

        // 4) ȸ�� �� ����
        var e = spinTarget.localEulerAngles;
        spinTarget.localEulerAngles = new Vector3(e.x, e.y, Mathf.Repeat(e.z, 360f));
        
        ctx.skillManager?.ExitCast();
    }
    
    public override void Execute(in SkillContext ctx)
    {
        StartCoroutine(DoFlipAndVolley(ctx));
    }
    
    protected override void OnArrowHit(in SkillContext ctx, Vector2 hitPoint, Collider2D hitCol)
    {
        
    }
}
