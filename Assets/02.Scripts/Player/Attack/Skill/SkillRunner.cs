using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRunner : MonoBehaviour
{
    [SerializeField] private SkillManager skillManager;

    // ��ư���� ȣ��
    public void OnClickSkill1() => skillManager.UseSkill("Skill1");
    public void OnClickSkill2() => skillManager.UseSkill("Skill2");
    public void OnClickSkill3() => skillManager.UseSkill("Skill3");
    public void OnClickSkill4() => skillManager.UseSkill("Skill4");
    public void OnClickSkill5() => skillManager.UseSkill("Skill5");
}
