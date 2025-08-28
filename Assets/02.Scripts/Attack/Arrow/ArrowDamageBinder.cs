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

    // 스킬마다 다른 값 주입하고 싶을 때 호출
    public void InitDamage(int damage) => Damage = damage;

    void HandleHit(Vector2 hitPoint, Collider2D other)
    {
        // 맞은 쪽에서 IDamageable 찾기(자식/부모 모두 커버)
        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            if (dmg == null || !dmg.IsAlive)
            {
                return;
            }
            
            dmg.TakeDamage(Damage);
        }
    }
}
