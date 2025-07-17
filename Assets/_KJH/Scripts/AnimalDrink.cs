using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class AnimalDrink : AnimalAbility
{
    [SerializeField] float drinkDistance = 0.5f;
    [SerializeField] float drinkTime = 5f;
    Collider[] colliders = new Collider[80];
    public override void Init()
    {
        //Debug.Log("공룡 Drink] 시작");
        StopCoroutine(nameof(GoToTarget));
        StartCoroutine(nameof(GoToTarget));
        agent.isStopped = false;
        target2 = FindAnyObjectByType<WaterBottle>();
        animal.petStateController.UpdateIsInteraction(true);
    }
    public override void UnInit()
    {
        base.UnInit();
        sfx?.Stop();
        StopCoroutine(nameof(GoToTarget));
        agent.isStopped = false;
        anim.SetInteger("animation", 1);
        animal.petStateController.UpdateIsInteraction(false);
    }
    WaterBowl target;
    WaterBottle target2;
    //SFX sfx;
    void FindTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, 50f, colliders, ~0, QueryTriggerInteraction.Ignore);
        int find = -1;
        target = null;
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].TryGetComponent(out WaterBowl bowl))
            {
                if (bowl.isPlaced)
                {
                    if (bowl.isPlaced && bowl.liquid.fillAmount >= Mathf.Lerp(bowl.fillRange.x, bowl.fillRange.y, 0.5f))
                    {
                        find = i;
                        break;
                    }
                }
            }
        }
        if (find == -1)
        {
            //Debug.Log("공룡 Drink] 주변에 placed 된 WaterBowl 오브젝트가 없습니다. Idle로 전환합니다.");
            animal.ChangeState(AnimalControl.State.Idle);
            return;
        }
        colliders[find].TryGetComponent(out target);
    }
    // 타겟으로 이동
    IEnumerator GoToTarget()
    {
        //Debug.Log("GoToTarget");
        FindTarget();
        yield return null;
        float distance = Vector3.Distance(target.transform.position, transform.position);
        if (distance < drinkDistance * 0.5f)
        {
            StartCoroutine(nameof(BackMoveToTarget));
            yield break;
        }
        agent.isStopped = false;
        bool result = agent.SetDestination(target.transform.position);
        float expectTime = distance / agent.speed;
        float startTime = Time.time;
        anim.SetInteger("animation", 21);
        while (true)
        {
            // 이동중......
            float sqrDistance = (target.transform.position - transform.position).sqrMagnitude;
            // 거리가 1.5m보다 가까워지거나.. expectTime의 1.5배보다 오래 걸릴경우(예를들어 벽에 끼여서 제자리 이동중인 경우) 루프 탈출
            if (sqrDistance <= drinkDistance * drinkDistance || Time.time - startTime > expectTime * 1.5f)
            {
                anim.SetInteger("animation", 1);
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                break;
            }
            if (!result)
            {
                result = agent.SetDestination(target.transform.position);
                yield return YieldInstructionCache.WaitForSeconds(0.1f);
                if (!result)
                {
                    //Debug.Log("공룡 Drink] 길찾기 실패. Idle로 전환합니다.");
                    animal.ChangeState(AnimalControl.State.Idle);
                    yield break;
                }
            }
            if (target == null || !target.gameObject.activeInHierarchy || !target.isPlaced)
            {
                //Debug.Log("공룡 Drink] '오브젝트가 파괴 되었거나' 또는 '플레이어가 Grab 했습니다'. Idle로 전환합니다.");
                animal.ChangeState(AnimalControl.State.Idle);
                yield break;
            }
            yield return null;
        }
        // 도착하고 0.2~0.4초간 잠깐 대기
        yield return null;
        StartCoroutine(nameof(LookTarget));
    }
    SFX sfx;
    // 만약 시작했을때부터 타겟과 너무 가까우면 살짝 뒤로 가기
    IEnumerator BackMoveToTarget()
    {
        //Debug.Log("BackMoveToTarget");
        // 뒤로 갈 지점 정하기
        agent.isStopped = false;
        Vector3 outer = (transform.position - target.transform.position).normalized;
        outer.y = 0f;
        Vector3 jitter = 0.2f * Random.onUnitSphere;
        jitter.y = 0f;
        outer += jitter;
        outer.Normalize();
        bool result = agent.SetDestination(transform.position + 3f * outer);
        for (int i = 0; i < 50; i++)
        {
            if (result) break;
            result = agent.SetDestination(transform.position + 3f * outer + Random.onUnitSphere);
        }
        yield return YieldInstructionCache.WaitForSeconds(1.5f);
        anim.SetInteger("animation", 1);
        agent.SetDestination(transform.position);
        agent.isStopped = true;
        StartCoroutine(nameof(LookTarget));
    }
    // 타겟에 도착한 다음 타겟쪽으로 천천히 고개 돌리기
    IEnumerator LookTarget()
    {
        animal.HeadIKLookPlayerOff();
        //Debug.Log("LookTarget");
        // 타겟을 향해 제자리에서 회전
        Vector3 targetForwardXZ = target.transform.position - transform.position;
        targetForwardXZ.y = 0f;
        float startTime = Time.time;
        while (Time.time - startTime < 3f)
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            float angle = Vector3.Angle(targetForwardXZ, forward);
            transform.forward = Vector3.Slerp(transform.forward, targetForwardXZ, 4f * Time.deltaTime);
            if (angle < 5) break;
            yield return null;
        }
        StartCoroutine(nameof(EatTarget));
    }
    // 타겟을 부드럽게 먹어가는 애니매이션 + 연출
    IEnumerator EatTarget()
    {
        //Debug.Log("EatTarget");
        // 혹시 모르니 먹기 직전 한번 더 타겟이 제대로 있는지 검사
        float distance = Vector3.Distance(target.transform.position, transform.position);
        if (distance > drinkDistance)
        {
            //Debug.Log("공룡 Drink] 향해서 이동 했으나 ' 너무 멀리있어서 닿지 않습니다. Idle로 전환합니다.");
            animal.ChangeState(AnimalControl.State.Idle);
            yield break;
        }
        if (target == null || !target.gameObject.activeInHierarchy || !target.isPlaced)
        {
            //Debug.Log("공룡 Drink] '오브젝트가 파괴 되었거나' 또는 '플레이어가 Grab 했습니다'. Idle로 전환합니다.");
            animal.ChangeState(AnimalControl.State.Idle);
            yield break;
        }

        // 먹는 애니매이션 재생 + 효과음 재생
        target2?.StopWaterFillOut();




        // 목마르지 않은 경우 처리
        if (animal.petStateController.currentState.thirsty < 8)
        {
            anim.SetInteger("animation", 12);
            sfx = AudioManager.Instance.PlayEffect("DinoNo", transform.position);
            yield return YieldInstructionCache.WaitForSeconds(2f);
            sfx?.Stop();
            target.Refuse();
        }
        else
        {
            anim.SetInteger("animation", 24);
            sfx = AudioManager.Instance.PlayEffect("Drink", transform.position);
            // Eat 애니매이션 길이에 따라 아래 시간 변경
            float startTime = Time.time;
            float range = target.fillRange.y - target.fillRange.x;
            float speed = (1f / drinkTime);
            ParticleManager.Instance.SpawnParticle(ParticleFlag.WaterSplash, transform.position, Quaternion.identity, null);
            Vector3 particlePos = transform.position + 0.2f * Vector3.up + 0.8f * transform.forward;
            int count = 0;
            while (Time.time - startTime < drinkTime)
            {
                if (Time.time - startTime > 0.2f && count == 0)
                {
                    count = 1;
                    ParticleManager.Instance.SpawnParticle(ParticleFlag.WaterSplash, particlePos, Quaternion.identity, null);
                }
                if (Time.time - startTime > 0.55f && count == 1)
                {
                    count = 2;
                    ParticleManager.Instance.SpawnParticle(ParticleFlag.WaterSplash, particlePos, Quaternion.identity, null);
                }
                if (Time.time - startTime > 0.7f && count == 2)
                {
                    count = 3;
                    ParticleManager.Instance.SpawnParticle(ParticleFlag.WaterSplash, particlePos, Quaternion.identity, null);
                }
                if (Time.time - startTime > 1.1f && count == 3)
                {
                    count = 4;
                    ParticleManager.Instance.SpawnParticle(ParticleFlag.WaterSplash, particlePos, Quaternion.identity, null);
                }
                // 여기에 각종 부드러운 처리들 구현
                target.liquid.fillAmount -= range * speed * Time.deltaTime;
                if (target.liquid.fillAmount <= target.fillRange.x - 0.02f)
                {
                    sfx?.Stop();
                    break;
                }
                if (target == null || !target.gameObject.activeInHierarchy || !target.isPlaced)
                {
                    sfx?.Stop();
                    //Debug.Log("공룡 Drink] 'Bowl오브젝트가 파괴 되었거나' 또는 '플레이어가 Grab 했습니다'. Idle로 전환합니다.");
                    animal.ChangeState(AnimalControl.State.Idle);
                    yield break;
                }
                yield return null;
            }
            anim.SetInteger("animation", 1);
            sfx?.Stop();
            // 먹기 완료
            animal.petStateController.Drink();
            //target.Reset();
            yield return YieldInstructionCache.WaitForSeconds(1f);
            sfx = AudioManager.Instance.PlayEffect("Drink(1)", transform.position);
            yield return YieldInstructionCache.WaitForSeconds(2f);
            sfx?.Stop();



        }



        // 모든 과정 완료후 30% 확률로 Idle 실행, 70% 확률로 Wander 실행
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


