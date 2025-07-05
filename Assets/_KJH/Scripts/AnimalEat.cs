using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class AnimalEat : AnimalAbility
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
        //Transform target = control.target;
        //yield return new WaitUntil(() => target != null);
        //agent.destination = target.position;
        float startTime = Time.time;
        yield return new WaitUntil(() => !agent.isStopped);
        Debug.Log("이동 끝");
    }
    
}
