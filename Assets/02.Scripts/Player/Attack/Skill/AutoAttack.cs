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
        // ȭ�� ���� �� ���� ����
        var go  = Instantiate(arrowPrefab, ctx.muzzle.position, Quaternion.identity);
        var proj = go.GetComponent<ArrowController>();

        var soInstance = Instantiate(trajectorySO);

        proj.BeginCollisionDelay(); // �� ���� ���� ����
        proj.SetupTrajectory(soInstance, ctx.muzzle.position, ctx.target); // target�� null�̾ ���� SO�� ó�� �����ϰ�
    }
}
