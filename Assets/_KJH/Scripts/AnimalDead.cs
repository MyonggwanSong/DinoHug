using System.Collections;
using UnityEngine;
public class AnimalDead : AnimalAbility
{
    public override void Init()
    {
        StartCoroutine(nameof(Activate));
    }
    public override void UnInit()
    {
        StopCoroutine(nameof(Activate));
    }
    IEnumerator Activate()
    {
        while (true)
        {
            yield return null;
        }
    }
    
}
