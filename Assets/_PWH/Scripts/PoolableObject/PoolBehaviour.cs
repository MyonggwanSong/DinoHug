using UnityEngine;

public abstract class PoolBehaviour : MonoBehaviour
{
    public void Despawn() => PoolManager.I.Despawn(this);
}