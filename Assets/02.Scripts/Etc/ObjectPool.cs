using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [System.Serializable]
    private class Pool
    {
        public GameObject prefab;
        public Queue<GameObject> queue = new Queue<GameObject>();
    }

    [SerializeField] private Transform poolRoot;

    private readonly Dictionary<GameObject, Pool> prefabToPool = new();
    private readonly Dictionary<GameObject, Pool> instanceToPool = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (poolRoot == null)
        {
            var go = new GameObject("[PoolRoot]");
            poolRoot = go.transform;
            poolRoot.SetParent(transform);
        }
    }

    public void Prewarm(GameObject prefab, int count)
    {
        var pool = GetOrCreatePool(prefab);
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab, poolRoot);
            obj.SetActive(false);
            pool.queue.Enqueue(obj);
            instanceToPool[obj] = pool;
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var pool = GetOrCreatePool(prefab);
        GameObject obj = null;

        while (pool.queue.Count > 0 && obj == null)
        {
            obj = pool.queue.Dequeue();
        }

        if (obj == null)
        {
            obj = Instantiate(prefab);
            instanceToPool[obj] = pool;
        }

        obj.transform.SetParent(parent);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        // 재사용 초기화 훅
        obj.GetComponent<ArrowCollision>()?.ResetForReuse();

        return obj;
    }

    public void Return(GameObject obj)
    {
        if (obj == null) return;

        if (!instanceToPool.TryGetValue(obj, out var pool))
        {
            // 미등록 객체면 그냥 비활성화만
            obj.SetActive(false);
            obj.transform.SetParent(poolRoot);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(poolRoot);
        pool.queue.Enqueue(obj);
    }

    private Pool GetOrCreatePool(GameObject prefab)
    {
        if (!prefabToPool.TryGetValue(prefab, out var pool))
        {
            pool = new Pool { prefab = prefab };
            prefabToPool[prefab] = pool;
        }
        return pool;
    }
}
