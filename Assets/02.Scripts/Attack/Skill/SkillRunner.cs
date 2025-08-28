using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRunner : MonoBehaviour
{
    [SerializeField] private SkillManager skillManager;

    // 버튼에서 호출
    public void OnClickSkill1() => skillManager.UseSkill("Skill_PullAnchor");
    public void OnClickSkill2() => skillManager.UseSkill("Skill_Backflip");
    public void OnClickSkill3() => skillManager.UseSkill("Skill_Silence");
    public void OnClickSkill4() => skillManager.UseSkill("Skill4");
    public void OnClickSkill5() => skillManager.UseSkill("Skill5");
}
