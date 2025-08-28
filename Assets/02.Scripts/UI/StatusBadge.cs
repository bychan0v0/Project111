using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBadge : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image fill;     // type=Filled
    [SerializeField] private Color readyColor = Color.white;
    [SerializeField] private Color tickingColor = new Color(1, .6f, .2f);

    float duration, remain;

    public void Setup(string text, float duration)
    {
        this.duration = Mathf.Max(0.0001f, duration);
        remain = this.duration;
        label.text = text;
        fill.fillAmount = 1f;
        fill.color = tickingColor;
    }

    private void Update()
    {
        remain -= Time.deltaTime;
        fill.fillAmount = Mathf.Clamp01(remain / duration);

        if (remain <= 0f)
        {
            // 끝나면 자연스런 정리
            fill.color = readyColor;
            Destroy(gameObject);
        }
    }

}
