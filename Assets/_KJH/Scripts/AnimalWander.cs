using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class AnimalWander : AnimalAbility
{

    Ray ray = new Ray();
    RaycastHit hit;
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
        yield return null;
        ray.direction = Vector3.down;
        Vector3 target = -999 * Vector3.one;
        for (int i = 0; i < 20; i++)
        {
            Vector3 random = transform.position + Random.Range(4f, 20f) * Random.insideUnitSphere;
            random.y = 100f;
            ray.origin = random;
            if (Physics.Raycast(ray, out hit, 200f, ~0, QueryTriggerInteraction.Ignore))
            {
                target = hit.point;
                break;
            }
        }
        if (target.x <= -999)
        {
            Debug.Log("Wander할 경로를 찾지 못했습니다.");
            yield return new WaitForSeconds(Random.Range(0f, 1f));
            animal.NextState();
            yield break;
        }
        agent.destination = target;
        yield return null;
        yield return null;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (agent.velocity == Vector3.zero) break;
        }
        yield return new WaitForSeconds(Random.Range(1.5f, 4f));
        animal.NextState();
    }

}
