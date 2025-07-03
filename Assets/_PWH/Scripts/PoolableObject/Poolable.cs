using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolBehaviour : MonoBehaviour
{
    public void Despawn() => PoolManager.Instance.Despawn(this);
}