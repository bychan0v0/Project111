using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour, ISkillBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private TrajectorySO trajectorySO;

    public void Execute(in SkillContext ctx)
    {
        // 화살 생성 및 궤적 세팅
        var go  = Instantiate(arrowPrefab, ctx.muzzle.position, Quaternion.identity);
        var proj = go.GetComponent<ArrowController>();

        var soInstance = Instantiate(trajectorySO);

        proj.BeginCollisionDelay(); // 네 기존 로직 유지
        proj.SetupTrajectory(soInstance, ctx.muzzle.position, ctx.target); // target이 null이어도 궤적 SO가 처리 가능하게
    }
}
