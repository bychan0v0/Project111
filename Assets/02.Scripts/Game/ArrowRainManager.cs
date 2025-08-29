using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRainManager : MonoBehaviour
{
    public static ArrowRainManager Instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject arrowPrefab;
    
    [Header("Trajectory")]
    [SerializeField] private TrajectorySO straightDownSO;
    
    [Header("Spawn Area")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private float spawnHeight = 12f;
    
    [Header("Pattern")]
    [SerializeField] private int arrowsPerVolley = 6;
    
    [Header("Ramp Up")]
    [SerializeField] private float volleyIntervalStart = 3.0f;
    [SerializeField] private float volleyIntervalMin   = 1.0f;
    [SerializeField] private float volleyIntervalAccel = 0.25f;
    
    private bool running;
    private Coroutine loopCo;
    
    private Transform aimDownTmp;
    
    private void Awake()
    {
        Instance = this;
        
        if (aimDownTmp == null)
        {
            var go = new GameObject("ArrowAimDownTmp");
            go.hideFlags = HideFlags.HideInHierarchy;
            aimDownTmp = go.transform;
        }
    }

    public void Begin()
    {
        if (running) return;
        running = true;
        loopCo = StartCoroutine(RainLoop());
    }
    
    public void Stop()
    {
        running = false;
        if (loopCo != null) StopCoroutine(loopCo);
    }
    
    private IEnumerator RainLoop()
    {
        float interval = volleyIntervalStart;
        var rng = new System.Random();

        while (running)
        {
            float lx = leftEdge.position.x;
            float rx = rightEdge.position.x;

            var xs = new List<float>(arrowsPerVolley);
            for (int i = 0; i < arrowsPerVolley; i++)
            {
                float x = Mathf.Lerp(lx, rx, (float)rng.NextDouble());
                xs.Add(x);
            }

            foreach (var x in xs)
            {
                Vector3 spawnPos = new Vector3(x, spawnHeight, 0f);
                var arrowObj = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
                var soInstance = Instantiate(straightDownSO);
                
                var ctrl = arrowObj.GetComponent<ArrowController>();
                if (ctrl != null && straightDownSO != null)
                {
                    aimDownTmp.position = new Vector3(x, spawnHeight - 100f, 0f);

                    ctrl.SetupTrajectory(soInstance, spawnPos, aimDownTmp);
                    ctrl.BeginCollisionDelay();
                }
            }

            yield return new WaitForSeconds(interval);
            interval = Mathf.Max(volleyIntervalMin, interval - volleyIntervalAccel);
        }
    }
}
