using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("Owner Refs")]
    [SerializeField] private Rigidbody2D rb;       // �� ������Ʈ�� �ٵ�
    [SerializeField] private Transform caster;     // ������ ��Ʈ
    [SerializeField] private Transform muzzle;     // �߻� ����(������ caster)
    [SerializeField] private Transform target;     // ���� Ÿ��(������ null)

    [Header("Loadout")]
    [SerializeField] private List<SkillData> loadout;

    private readonly Dictionary<string, ISkillBehaviour> skills = new();
    private readonly Dictionary<string, float> cooldownEnd = new();

    private void Awake()
    {
        // ��ų ������ �̸� ����(�÷��̾�/AI ��� ����)
        foreach (var def in loadout)
        {
            var obj = Instantiate(def.prefab, transform);
            var beh = obj.GetComponent<ISkillBehaviour>();
            skills[def.skillId] = beh;
            cooldownEnd[def.skillId] = 0f;
        }
    }
    
    private int castDepth;
    
    public bool IsCasting => castDepth > 0;
    public void EnterCast() { castDepth++; }
    public void ExitCast()  { if (castDepth > 0) castDepth--; }
    
    public bool UseSkill(string skillId, float chargeTime = 0f, Vector2 aimFallback = default)
    {
        if (!skills.TryGetValue(skillId, out var beh)) return false;

        // ��ٿ� üũ
        var def = loadout.Find(d => d.skillId == skillId);
        if (def)
        {
            if (Time.time < cooldownEnd[skillId]) return false;
            cooldownEnd[skillId] = Time.time + def.cooldown;
        }

        var ctx = new SkillContext
        {
            rb = rb,
            caster = caster,
            muzzle = muzzle,
            target = target,
            aimDir = aimFallback,
            chargeTime = chargeTime,
            
            skillManager = this
        };

        beh.Execute(in ctx);
        return true;
    }
}
