using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowImpactVFX : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private GameObject impactPrefab;

    [Header("Spawn Filter")]
    [SerializeField] private LayerMask playerMask;

    [Header("Normal Guess")]
    [SerializeField] private float rayBias = 0.15f;    // 표면 노말 추정용 짧은 레이

    private ArrowController ctrl;
    private Rigidbody2D rb;
    private Transform tr;

    private void Awake()
    {
        ctrl = GetComponent<ArrowController>();
        rb   = GetComponent<Rigidbody2D>();
        tr   = transform;
    }

    private void OnEnable()  { ctrl.OnFirstHit += SpawnImpact; }
    private void OnDisable() { ctrl.OnFirstHit -= SpawnImpact; }

    private void SpawnImpact(Vector2 hitPoint, Collider2D other)
    {
        if (!impactPrefab || !other) return;

        bool isPlayerByLayer = (playerMask.value & (1 << other.gameObject.layer)) != 0;
        bool isPlayerByComp  = other.GetComponentInParent<PlayerHp>() != null;
        if (!(isPlayerByLayer || isPlayerByComp))
            return;

        Vector3 pos = new Vector3(hitPoint.x, hitPoint.y, 0f);

        Vector2 dir = rb ? rb.velocity.normalized : (Vector2)tr.right;
        Vector2 normal = -dir;
        if (other)
        {
            int mask = 1 << other.gameObject.layer;
            var hit = Physics2D.Raycast(hitPoint - dir * rayBias, dir, rayBias * 2f, mask);
            if (hit.collider) normal = hit.normal;
        }

        Quaternion rot = Quaternion.FromToRotation(Vector3.up, new Vector3(normal.x, normal.y, 0f));

        var fx = Instantiate(impactPrefab, pos, rot);

        fx.transform.SetParent(other.transform, true);

        var ps = fx.GetComponent<ParticleSystem>();
        if (ps)
        {
            var main = ps.main;
            float life = main.duration + main.startLifetime.constantMax;
            Destroy(fx, life);
        }
        else
        {
            Destroy(fx, 1.5f);
        }
    }
}
