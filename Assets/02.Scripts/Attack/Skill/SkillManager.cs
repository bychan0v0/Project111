using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("Owner Refs")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform caster;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform target;

    [Header("Loadout")]
    [SerializeField] private List<SkillData> loadout;

    private readonly Dictionary<string, ISkillBehaviour> skills = new();
    private readonly Dictionary<string, float> cooldownEnd = new();

    private PlayerController player;
    
    private bool isSilence = false;

    public IReadOnlyList<SkillData> Loadout => loadout;
    
    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        
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
    public void SetSilence() { isSilence = true; }
    public void ResetSilence() { isSilence = false; }
    public void EnterCast() { castDepth++; }
    public void ExitCast()  { if (castDepth > 0) castDepth--; }
    
    public bool UseSkill(string skillId, float chargeTime = 0f, Vector2 aimFallback = default)
    {
        if (!skills.TryGetValue(skillId, out var beh)) return false;
        if (!player.IsGround)
        {
            Debug.Log("플레이어가 땅에 있지 않습니다.");
            return false;
        }
        if (isSilence)
        {
            Debug.Log("플레이어가 침묵 상태입니다.");
            return false;
        }
        if (player.IsRoot)
        {
            Debug.Log("플레이어가 속박 중입니다.");
            return false;
        }
        
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
    
    public bool TryUseFirstReadyInLoadout()
    {
        foreach (var s in loadout)
        {
            if (UseSkill(s.skillId))
                return true;
        }
        return false;
    }
    
    public float GetCooldownRemaining(string skillId)
    {
        if (!cooldownEnd.TryGetValue(skillId, out var end)) return 0f;
        return Mathf.Max(0f, end - Time.time);
    }
}
