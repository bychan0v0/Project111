using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Skill4Action : MonoBehaviour, ISkillBehaviour
{
    [Header("Refs")]
    [SerializeField] private TrajectorySO trajectorySO;
    
    private AttackManager attackManager;
    private GameObject arrowPrefab;
    
    private void Awake()
    {
        attackManager = GetComponentInParent<AttackManager>();
        arrowPrefab = attackManager.GetArrowPrefab();
    }
    
    public void Execute(in SkillContext ctx)
    {
        
    }
}
