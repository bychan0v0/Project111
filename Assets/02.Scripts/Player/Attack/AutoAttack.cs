using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject arrowPrefab;

    [Header("Fire Settings")]
    [SerializeField] private float attackInterval = 0.6f;
    [SerializeField] private float idleEps = 0.02f;

    [Header("Trajectory SO")]
    [SerializeField] private TrajectorySO bezierSO;

    private Rigidbody2D rb;
    private float timer;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        if (!muzzle) muzzle = transform;
    }

    private void Update()
    {
        if (!enemy || !arrowPrefab || !bezierSO) return;

        timer += Time.deltaTime;
        bool isIdle = !rb || Mathf.Abs(rb.velocity.x) <= idleEps;

        if (isIdle && timer >= attackInterval)
        {
            FireOnce();
            timer = 0f;
        }
    }

    private void FireOnce()
    {
        Vector3 start = muzzle.position;
        Vector3 aim = enemy.position;

        var go = Instantiate(arrowPrefab, start, Quaternion.identity);
        var projCol = go.GetComponent<ArrowCollision>();
        var proj = go.GetComponent<ArrowController>();
        var soInstance = Instantiate(bezierSO);

        projCol.BeginCollisionDelay();
        proj.SetupTrajectory(soInstance, start, aim, enemy);
    }
}