using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [SerializeField] private SkillManager skillManager;
    
    [SerializeField] private List<Image> fills;
    [SerializeField] private List<TMP_Text> labels;

    [Header("Display")]
    [SerializeField] private bool fillShowsRemaining = true;
    [SerializeField] private bool hideTextWhenReady = true;
    [SerializeField] private string readyText = "";
    [SerializeField] private float uiUpdateHz = 20f;

    private float tick;

    private void Update()
    {
        tick -= Time.deltaTime;
        if (tick > 0f) return;
        tick = 1f / Mathf.Max(1f, uiUpdateHz);

        var loadout = skillManager?.Loadout;
        if (loadout == null) return;

        int n = loadout.Count;
        for (int i = 0; i < fills.Count || i < labels.Count; i++)
        {
            if (i >= n)
            {
                if (i < fills.Count && fills[i]) fills[i].fillAmount = fillShowsRemaining ? 0f : 1f;
                if (i < labels.Count && labels[i]) labels[i].text = "";
                continue;
            }

            var def = loadout[i];
            if (def == null) continue;

            float total   = Mathf.Max(0.0001f, def.cooldown);
            float remain  = skillManager.GetCooldownRemaining(def.skillId);
            float ratio   = Mathf.Clamp01(remain / total);
            float fillVal = fillShowsRemaining ? ratio : (1f - ratio);

            if (i < fills.Count && fills[i])
                fills[i].fillAmount = fillVal;

            if (i < labels.Count && labels[i])
            {
                if (remain > 0f)
                {
                    labels[i].text = $"{remain:0}";
                    labels[i].enabled = true;
                }
                else
                {
                    if (hideTextWhenReady) { labels[i].text = readyText; labels[i].enabled = !string.IsNullOrEmpty(readyText); }
                    else                   { labels[i].text = readyText; labels[i].enabled = true; }
                }
            }
        }
    }
}
