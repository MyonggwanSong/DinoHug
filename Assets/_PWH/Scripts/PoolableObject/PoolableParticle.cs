using DG.Tweening;
using UnityEngine;

public class PoolableParticle : PoolBehaviour
{
    [SerializeField] ParticleSystem particle;


    [Header("Duration")]
    [SerializeField] float duration;

    void OnEnable()
    {
        particle.Play();

        DOVirtual.DelayedCall(duration, () => Despawn());
    }
}