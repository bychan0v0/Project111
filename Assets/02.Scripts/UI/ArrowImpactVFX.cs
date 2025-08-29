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

        // 플레이어일 때만 생성
        bool isPlayerByLayer = (playerMask.value & (1 << other.gameObject.layer)) != 0;
        bool isPlayerByComp  = other.GetComponentInParent<PlayerHp>() != null;
        if (!(isPlayerByLayer || isPlayerByComp))
            return;

        // 1) 위치
        Vector3 pos = new Vector3(hitPoint.x, hitPoint.y, 0f);

        // 2) 표면 노말 추정 (없으면 -비행방향)
        Vector2 dir = rb ? rb.velocity.normalized : (Vector2)tr.right;
        Vector2 normal = -dir;
        if (other)
        {
            int mask = 1 << other.gameObject.layer;
            var hit = Physics2D.Raycast(hitPoint - dir * rayBias, dir, rayBias * 2f, mask);
            if (hit.collider) normal = hit.normal;
        }

        // 3) 회전
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, new Vector3(normal.x, normal.y, 0f));

        // 4) 생성
        var fx = Instantiate(impactPrefab, pos, rot);

        // 5) 맞은 대상에 붙일지(원하면 유지)
        fx.transform.SetParent(other.transform, true);

        // 6) 자동 파괴
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
