using System.Collections;
using UnityEngine;
public class AnimalDrink : AnimalAbility
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
        if (animal.target != null && animal.target.isPlace)
        animal.target.root.gameObject.SetActive(false);
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        animal.NextState();
    }
    
}
