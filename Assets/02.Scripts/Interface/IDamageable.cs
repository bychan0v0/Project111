using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool IsAlive { get; }
    void TakeDamage(int amount);
}
