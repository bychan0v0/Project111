using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private GameObject impactFxPrefab;
    
    [Header("Fall")]
    [SerializeField] private float fallSpeed = 25f;

    [Header("Blocker")]
    [SerializeField] private float blockerSkin = 1f;

    private Rigidbody2D rb;
    
    private Vector3 target;
    private BoxCollider2D zoneCol;            // DangerZone 트리거 (정답 범위)
    private LayerMask playerMask;
    private LayerMask groundMask;
    private System.Action onAfterImpact;

    private bool falling;

    public void Init(
        Vector3 targetPos,
        BoxCollider2D zoneCollider,
        LayerMask playerMask,
        LayerMask groundMask,
        System.Action onAfterImpact)
    {
        target = targetPos;
        zoneCol = zoneCollider;
        this.playerMask = playerMask;
        this.groundMask = groundMask;
        this.onAfterImpact = onAfterImpact;
        falling = true;
    }

    private void Awake() => rb = GetComponent<Rigidbody2D>();
    
    private void FixedUpdate()
    {
        if (!falling) return;
        var step = fallSpeed * Time.fixedDeltaTime;
        rb.MovePosition(Vector2.MoveTowards(rb.position, target, step));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LayerMask impactMask = groundMask | playerMask;
        if (!IsInMask(collision.collider.gameObject.layer, impactMask)) return;
        
        if (!falling) return;
        falling = false;

        Vector2 hitPoint = collision.GetContact(0).point;
        var fx = Instantiate(impactFxPrefab, hitPoint, Quaternion.identity);
        var ps = fx.GetComponent<ParticleSystem>();
        if (ps)
        {
            var main = ps.main;
            float life = main.duration + main.startLifetime.constantMax;
            Destroy(fx, life);
        }
        
        // 충돌 위치를 DangerZone 중심으로 스냅(시각/판정 정렬)
        var zb = zoneCol.bounds;
        transform.position = zb.center;

        // === 1) 플레이어 즉사 ===
        var list = new List<Collider2D>();
        var pf = new ContactFilter2D { useTriggers = false };
        pf.SetLayerMask(playerMask);
        zoneCol.OverlapCollider(pf, list);
        foreach (var c in list)
        {
            var hp = c.GetComponent<PlayerHp>();
            if (hp != null) hp.TakeDamage(99999);
        }
        list.Clear();

        // === 2) Ground 파괴 (오브젝트형) ===
        var gf = new ContactFilter2D { useTriggers = false };
        gf.SetLayerMask(groundMask);
        zoneCol.OverlapCollider(gf, list);
        foreach (var g in list)
        {
            if (g != null) Destroy(g.gameObject);
        }
        list.Clear();

        // === 3) 투명 벽(Blocker) 생성 ===
        CreateInvisibleBlocker(zb);

        onAfterImpact?.Invoke();
        Destroy(gameObject);
    }

    private void CreateInvisibleBlocker(Bounds worldBounds)
    {
        // 살짝 확장해서 미세한 틈 방지
        var size = worldBounds.size + new Vector3(blockerSkin, blockerSkin + 10f, 0f);
        var center = worldBounds.center;

        GameObject blocker = new GameObject("MeteorVoidBlocker");

        // 위치/스케일(부모 스케일 영향 방지)
        blocker.transform.SetParent(null);
        blocker.transform.position = new Vector3(center.x, center.y, 0f);
        blocker.transform.rotation = Quaternion.identity;
        blocker.transform.localScale = Vector3.one;

        // 실제 충돌체
        var bc = blocker.AddComponent<BoxCollider2D>();
        bc.isTrigger = false;
        bc.size = size; // 월드 기준 크기
    }
    
    private bool IsInMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
}
