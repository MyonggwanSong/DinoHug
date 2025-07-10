using DG.Tweening;
using UnityEngine;

public class PoolableParticle : PoolBehaviour
{
    [SerializeField] ParticleSystem particle;

    void OnEnable()
    {
        particle.Play();
    }

    void Update()
    {
        if (particle.isStopped)
        {
            Despawn();
        }
    }
}