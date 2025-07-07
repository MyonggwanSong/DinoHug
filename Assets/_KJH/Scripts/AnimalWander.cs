using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AnimalWander : AnimalAbility
{
    NavMeshAgent agent;
    RaycastHit hit;
    Ray ray = new Ray();
    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out agent);
    }
    public override void Init()
    {
        Debug.Log("공룡 Wander 시작");
        StartCoroutine(Wander());
    }
    public override void UnInit()
    {
        //Debug.Log("공룡 Wander 끝");
    }
    IEnumerator Wander()
    {
        ray.direction = Vector3.down;
        Vector3 target = 999 * Vector3.one;
        for (int i = 0; i < 100; i++)
        {
            Vector3 randomPos = transform.position + 30f * Random.insideUnitSphere;
            randomPos.y = 100f;
            ray.origin = randomPos;
            //Debug.DrawRay(ray.origin, 200f * ray.direction, Color.white, 5f);
            if (Physics.Raycast(ray, out hit, 200f, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.layer == 3)
                {
                    target = hit.point;
                    break;
                }
            }
            yield return null;
        }
        if (target.x >= 999)
        {
            //Debug.Log("공룡 Wander --> 100번의 땅 검사를 했는데도 target을 찾지 못했습니다. Idle로 전환합니다.");
            animal.ChangeState(AnimalControl.State.Idle);
            yield break;
        }
        agent.destination = target;
        float expectTime = Vector3.Distance(target, transform.position) / agent.speed;
        float startTime = Time.time;
        while (true)
        {
            // 이동중......
            float sqrDistance = (target - transform.position).sqrMagnitude;
            // 거리가 0.1 보다 가까워지거나.. expectTime의 1.5배보다 오래 걸릴경우(예를들어 벽에 끼여서 제자리 이동중인 경우) 루프 탈출
            if (sqrDistance <= 0.01f || Time.time - startTime > expectTime * 1.5f)
            {
                break;
            }
            yield return null;
        }
        // 도착하고 1~2초간 잠깐 대기
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        // 목적지에 도착해서 정상적인 종료일시
        // 30% 확률로 Idle 실행, 70% 확률로 Wander 실행
        if (Random.value < 0.3f)
        {
            animal.ChangeState(AnimalControl.State.Idle);
        }
        else
        {
            animal.ChangeState(AnimalControl.State.Wander);
        }
    }

    
    
}
