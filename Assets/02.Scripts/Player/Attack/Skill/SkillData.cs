using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Data")]

public class SkillData : ScriptableObject
{
    public string skillId;    // ����ũ Ű
    public float cooldown = 1f;    // ��ٿ�
    public GameObject prefab;    // ������ ������ (����ü ��)
}
    