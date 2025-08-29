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
    private BoxCollider2D zoneCol;
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
        
        CameraShaker.Instance?.Begin(0.12f, 18f);
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

        CameraShaker.Instance?.OneShot(0.45f,  0.22f, 22f);
        CameraShaker.Instance?.Stop();
        
        Vector2 hitPoint = collision.GetContact(0).point;
        var fx = Instantiate(impactFxPrefab, hitPoint, Quaternion.identity);
        var ps = fx.GetComponent<ParticleSystem>();
        if (ps)
        {
            var main = ps.main;
            float life = main.duration + main.startLifetime.constantMax;
            Destroy(fx, life);
        }
        
        var zb = zoneCol.bounds;
        transform.position = zb.center;

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

        var gf = new ContactFilter2D { useTriggers = false };
        gf.SetLayerMask(groundMask);
        zoneCol.OverlapCollider(gf, list);
        foreach (var g in list)
        {
            if (g != null) Destroy(g.gameObject);
        }
        list.Clear();

        CreateInvisibleBlocker(zb);

        onAfterImpact?.Invoke();
        Destroy(gameObject);
    }

    private void CreateInvisibleBlocker(Bounds worldBounds)
    {
        var size = worldBounds.size + new Vector3(blockerSkin, blockerSkin + 10f, 0f);
        var center = worldBounds.center;

        GameObject blocker = new GameObject("MeteorVoidBlocker");

        blocker.transform.SetParent(null);
        blocker.transform.position = new Vector3(center.x, center.y, 0f);
        blocker.transform.rotation = Quaternion.identity;
        blocker.transform.localScale = Vector3.one;

        var bc = blocker.AddComponent<BoxCollider2D>();
        bc.isTrigger = false;
        bc.size = size;
    }
    
    private bool IsInMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
}
