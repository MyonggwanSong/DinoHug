using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AnimalCall : AnimalAbility
{
    [Header("Value")]
    [SerializeField] float callDistance;                // Player와의 거리
    [SerializeField] float callWaitTime;                // Player에게 도착 후 대기시간. => 대기 시간을 넘어가면 다시 wandor 상태로 넘어가기
    [ReadOnlyInspector] public bool isArrived;

    [Header("Player Reference")]
    [ReadOnlyInspector, SerializeField] Transform target;                  // Player Reference

    public override void Init()
    {
        // 상태 시작
        isArrived = false;
        agent.isStopped = false;
        target = Camera.main.transform;

        Debug.Log($"Init Call.");
        StopCoroutine(nameof(FollowTarget));
        StartCoroutine(nameof(FollowTarget));
    }

    public override void UnInit()
    {
        base.UnInit();
        Debug.Log($"UnInit Call.");
        StopCoroutine(nameof(FollowTarget));
        agent.isStopped = false;
    }

    // 동물이 호출을 받았다면. 실행할 것들
    IEnumerator FollowTarget()
    {
        if (target == null) yield break;

        yield return null;
        Debug.Log("[Animal Call] : Player 호출 인지");
        yield return new WaitForSeconds(0.5f);

        Debug.Log("[Animal Call] : Player에게 다가가기");

        // 거리가 좁혀 질 때까지 다가가기
        while (Vector3.Distance(gameObject.transform.position, target.transform.position) > callDistance)
        {
            bool check = agent.SetDestination(target.transform.position + (Vector3.forward * 2f));

            if (!check)
            {
                Debug.Log("목적지 찾기 실패...");
            }

            Debug.Log($"[Animal Call] : Player에게 다가가는 중...... {Vector3.Distance(gameObject.transform.position, target.transform.position)}");

            yield return new WaitForSeconds(0.1f);
        }

        agent.isStopped = true;
        isArrived = true;

        Debug.Log("Target에 도착했습니다.");

        // 도착한 상태 멈추기
        yield return new WaitForSeconds(callWaitTime);

        //이 시간 이후에도 동물의 상태가 Call이라면. Idle 상태로 넘어가기
        if (animal.state.Equals(AnimalControl.State.CallIdle))
        {
            Debug.Log("시간 초과... Idle 상태로 이동합니다.");
            animal.ChangeState(AnimalControl.State.Idle);
        }
    }
}


/*
    1. 인지
    2. 이모티콘 띄우기
    3. 돌아보기, 다가가기
*/