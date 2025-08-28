using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHp : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1)] private int maxHP = 100;
    public int CurrentHP { get; private set; }
    public int MaxHP => maxHP;
    public bool IsAlive => CurrentHP > 0;

    public event Action<int,int> OnChanged;
    public event Action OnDied;

    private void Awake()
    {
        CurrentHP = maxHP;
        OnChanged?.Invoke(CurrentHP, maxHP);
    }

    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;
        CurrentHP = Mathf.Max(0, CurrentHP - Mathf.Max(0, amount));
        OnChanged?.Invoke(CurrentHP, maxHP);
        if (CurrentHP == 0) OnDied?.Invoke();
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;
        CurrentHP = Mathf.Min(maxHP, CurrentHP + Mathf.Max(0, amount));
        OnChanged?.Invoke(CurrentHP, maxHP);
    }
}
