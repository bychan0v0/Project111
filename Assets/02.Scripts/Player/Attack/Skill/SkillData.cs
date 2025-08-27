using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Data")]

public class SkillData : ScriptableObject
{
    public string skillId;    // 유니크 키
    public float cooldown = 1f;    // 쿨다운
    public GameObject prefab;    // 실행할 프리팹 (투사체 등)
}
    