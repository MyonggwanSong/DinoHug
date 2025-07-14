using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AnimalEat : AnimalAbility
{
    [SerializeField] float eatDistance = 1.5f;
    Collider[] colliders = new Collider[80];

    public override void Init()
    {        
        //Debug.Log("공룡 Eat] 시작");
        StopCoroutine(nameof(GoToFood));
        StartCoroutine(nameof(GoToFood));
        agent.isStopped = false;
    }
    public override void UnInit()
    {
        base.UnInit();
        sfx?.Stop();
        StopCoroutine(nameof(GoToFood));
        agent.isStopped = true;
        if (target != null)
        {
            target.EnableGrab();
        }
    }
    Food target = null;
    IEnumerator GoToFood()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, 50f, colliders, ~0, QueryTriggerInteraction.Ignore);
        int find = -1;
        target = null;
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].TryGetComponent(out Food food))
            {
                if (food.isPlaced)
                {
                    find = i;
                    break;
                }
            }
        }
        if (find == -1)
        {
            //Debug.Log("공룡 Eat] 주변에 placed 된 Food 오브젝트가 없습니다. Idle로 전환합니다.");
            animal.ChangeState(AnimalControl.State.Idle);
            yield break;
        }
        colliders[find].TryGetComponent(out target);
        agent.destination = target.transform.position;
        float expectTime = Vector3.Distance(target.transform.position, transform.position) / agent.speed;
        float startTime = Time.time;
        anim.CrossFade("Move", 0.1f);
        agent.isStopped = false;
        while (true)
        {
            // 이동중......
            float sqrDistance = (target.transform.position - transform.position).sqrMagnitude;
            // 거리가 1.5m보다 가까워지거나.. expectTime의 1.5배보다 오래 걸릴경우(예를들어 벽에 끼여서 제자리 이동중인 경우) 루프 탈출
            if (sqrDistance <= eatDistance * eatDistance || Time.time - startTime > expectTime * 1.5f)
            {
                anim.CrossFade("Idle", 0.1f);
                agent.isStopped = true;
                break;
            }
            if (target == null || !target.gameObject.activeInHierarchy || !target.isPlaced)
            {
                //Debug.Log("공룡 Eat] 'Food 오브젝트가 파괴 되었거나' 또는 '플레이어가 Grab 했습니다'. Idle로 전환합니다.");
                animal.ChangeState(AnimalControl.State.Idle);
                yield break;
            }
            yield return null;
        }
        // 도착하고 0.2~0.4초간 잠깐 대기
        yield return YieldInstructionCache.WaitForSeconds(Random.Range(0.2f, 0.4f));
        // 타겟을 향해 제자리에서 회전
        Vector3 targetForwardXZ = target.transform.position - transform.position;
        targetForwardXZ.y = 0f;
        while (true)
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            float angle = Vector3.Angle(targetForwardXZ, forward);
            transform.forward = Vector3.Slerp(transform.forward, targetForwardXZ, 4f * Time.deltaTime);
            if (angle < 5) break;
            yield return null;
        }
        float distance = Vector3.Distance(target.transform.position, transform.position);
        if (distance > eatDistance)
        {
            //Debug.Log("공룡 Eat] Food를 향해서 이동 했으나 'Food가 너무 멀리있어서 닿지 않습니다. Idle로 전환합니다.");
            animal.ChangeState(AnimalControl.State.Idle);
            yield break;
        }
        if (target == null || !target.gameObject.activeInHierarchy || !target.isPlaced)
        {
            //Debug.Log("공룡 Eat] 'Food 오브젝트가 파괴 되었거나' 또는 '플레이어가 Grab 했습니다'. Idle로 전환합니다.");
            animal.ChangeState(AnimalControl.State.Idle);
            yield break;
        }
        // 먹는 애니매이션 재생 + 먹고있는 동안 XR그랩 못하게
        anim.CrossFade("Eat", 0.1f);
        // 효과음 재생
        sfx = AudioManager.Instance.PlayEffect("EatMeat", transform.position, 1.0f);
        target.DisableGrab();
        // Eat 애니매이션 길이에 따라 아랫줄 시간 변경
        yield return YieldInstructionCache.WaitForSeconds(3f);
        // 여기에 Food 오브젝트를 최초 상태로 리셋 처리
        sfx?.Stop();
        target.Reset();
        // 여기에 배고픔 게이지 하강 처리
        //Debug.Log("공룡 Eat] 'Food 먹기 성공. 이 줄에서 배고픔 게이지 감소 처리");
        animal.petStateController.Feed();
        // 모든 과정 완료후 정상적인 종료일시
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
    SFX sfx;



}
