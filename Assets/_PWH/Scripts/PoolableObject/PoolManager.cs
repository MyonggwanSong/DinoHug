using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : BehaviourSingleton<PoolManager>
{
    protected override bool IsDontDestroy() => false;

    //Key(instance), Value(key)
    Dictionary<PoolBehaviour, PoolBehaviour> pools;
    Dictionary<PoolBehaviour, ObjectPool<PoolBehaviour>> poolDic;

    protected override void Awake()
    {
        base.Awake();

        pools = new();
        poolDic = new();
    }

    // pool은 Prefab
    public void CreatePool(PoolBehaviour pool, int size = 30)
    {
        // if (pools.Values..Contains(pool)) return;               //이미 저장 되어있는 pool이면 무시
        if (pools.Values.ToList().Contains(pool)) return;

        Debug.Log("Create Pool!");

        var poolInstance = new ObjectPool<PoolBehaviour>(
            createFunc: () =>
            {
                PoolBehaviour p = Instantiate(pool);
                pools[p] = pool;
                return p;
            },
            actionOnGet: (v) =>
            {
                v.gameObject.SetActive(true);
            },
            actionOnRelease: (v) =>
            {
                v.gameObject.SetActive(false);
            },
            actionOnDestroy: (v) =>
            {
                Destroy(v.gameObject);
            },
            maxSize: size);

        poolDic[pool] = poolInstance;
    }

    //pool은 prefab으로 Key가 된다.
    public PoolBehaviour Spawn(PoolBehaviour pool, Vector3 position, Quaternion rot, Transform parent)
    {
        if (!pools.Values.ToList().Contains(pool))
        {
            CreatePool(pool, 20);
        }

        var instance = poolDic[pool].Get();

        instance.transform.position = position;
        instance.transform.rotation = rot;
        instance.transform.SetParent(parent ?? transform, true);
        instance.transform.localScale = Vector3.one * 0.5f;
        return instance;
    }

    // 여기서의 매개 변수는 instance
    public void Despawn(PoolBehaviour pool)
    {
        if (pool == null || !pools.Keys.ToList().Contains(pool)) return;

        var keyPool = pools[pool];

        poolDic[keyPool].Release(pool);
        pool.gameObject.SetActive(false);
    }
}