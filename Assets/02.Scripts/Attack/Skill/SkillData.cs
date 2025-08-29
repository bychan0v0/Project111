using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Data")]

public class SkillData : ScriptableObject
{
    public string skillId;
    public float cooldown = 1f;
    public GameObject prefab;
}
    