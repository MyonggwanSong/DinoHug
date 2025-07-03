using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class AnimalGoTo : AnimalAbility
{
    NavMeshAgent agent;
    public override void Init()
    {
        TryGetComponent(out agent);
        StartCoroutine(nameof(Activate));
    }
    public override void UnInit()
    {
        StopCoroutine(nameof(Activate));
    }
    IEnumerator Activate()
    {
        
        Transform target = animal.target.root;
        yield return new WaitUntil(() => target != null);
        agent.destination = target.position;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (agent.velocity == Vector3.zero) break;
            if (animal.target == null || !animal.target.isPlace) break;
        }
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        animal.NextState();
    }
    
}
