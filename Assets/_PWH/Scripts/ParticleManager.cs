using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ParticleFlag
{
    Eating,
    Drinking,
    Petting,
    Dust,
    DustSmall,
    Twinkle,
    WaterSplash,
    Confetti1,
    Confetti2,
}

[Serializable]
public struct ParticleData
{
    public ParticleFlag flag;
    public PoolableParticle pb;
}

public class ParticleManager : BehaviourSingleton<ParticleManager>
{
    protected override bool IsDontDestroy() => true;

    [Header("Particle")]
    [SerializeField] List<ParticleData> particles;

    public PoolableParticle SpawnParticle(ParticleFlag flag, Vector3 position, Quaternion rot, Transform parent)
    {
        PoolableParticle pb = particles.Find(p => p.flag.Equals(flag)).pb;

        if (pb == null) return null;

        PoolManager.Instance.Spawn(pb, position, rot, parent);

        return pb;
    }
}