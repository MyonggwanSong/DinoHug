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
        // Transform target = control.target;
        // yield return new WaitUntil(() => target != null);
        // agent.destination = target.position;
        yield return null;
        yield return null;
        while (true)
        {
            yield return null;
            Debug.Log(agent.velocity);
            if (agent.velocity == Vector3.zero) break;
        }
        
        
    }
    
}
