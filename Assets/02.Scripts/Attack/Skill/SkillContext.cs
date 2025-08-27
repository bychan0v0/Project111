using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillContext
{
    public Rigidbody2D rb;
    public Transform caster;
    public Transform muzzle;
    public Transform target;
    public Vector2 aimDir;
    public float chargeTime;
    
    public SkillManager skillManager;
}
