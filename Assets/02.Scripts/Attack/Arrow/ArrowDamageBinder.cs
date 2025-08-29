using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDamageBinder : MonoBehaviour
{
    [SerializeField, Min(1)] int defaultDamage = 50;

    public int Damage { get; private set; }
    ArrowController arrowController;

    void Awake()
    {
        arrowController = GetComponent<ArrowController>();
        Damage = defaultDamage;
    }

    void OnEnable()  => arrowController.OnFirstHit += HandleHit;
    void OnDisable() => arrowController.OnFirstHit -= HandleHit;

    public void InitDamage(int damage) => Damage = damage;

    void HandleHit(Vector2 hitPoint, Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            if (dmg == null || !dmg.IsAlive)
            {
                return;
            }
            
            dmg.TakeDamage(Damage);
            HitUIRoot.Instance?.ShowDamage(Damage, hitPoint);
        }
    }
}
