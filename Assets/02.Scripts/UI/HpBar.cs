using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private PlayerHp health;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text label;

    void OnEnable()
    {
        health.OnChanged += Refresh;
        Refresh(health.CurrentHP, health.MaxHP);
    }

    void OnDisable()
    {
        health.OnChanged -= Refresh;
    }

    void Refresh(int cur, int max)
    {
        slider.value = (float)cur / max;
        label.text = $"{cur}";
    }
}
