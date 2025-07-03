using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : BehaviourSingleton<PoolManager>
{
    protected override bool IsDontDestroy() => false;

    List<PoolBehaviour> pools;
    Dictionary<PoolBehaviour, ObjectPool<PoolBehaviour>> poolDic;

    protected override void Awake()
    {
        base.Awake();

        pools = new();
        poolDic = new();
    }

    public void CreatePool(PoolBehaviour pool, int size = 10)
    {
        if (pools.Contains(pool)) return;               //이미 저장 되어있는 pool이면 무시

        var poolInstance = new ObjectPool<PoolBehaviour>(
            createFunc: () =>
            {
                PoolBehaviour p = Instantiate(pool);
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

    public PoolBehaviour Spawn(PoolBehaviour pool, Vector3 position, Quaternion rot, Transform parent)
    {
        if (!pools.Contains(pool)) return null;

        var instance = poolDic[pool].Get();

        instance.transform.position = position;
        instance.transform.rotation = rot;
        instance.transform.SetParent(parent ?? transform, true);

        return instance;
    }

    public void Despawn(PoolBehaviour pool)
    {
        if (pool == null && !pools.Contains(pool)) return;

        poolDic[pool].Release(pool);
        pool.gameObject.SetActive(false);
    }
}