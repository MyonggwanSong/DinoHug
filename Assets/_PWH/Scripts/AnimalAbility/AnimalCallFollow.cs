using System.Collections;
using UnityEngine;

public class AnimalCallFollow : AnimalAbility
{
    [Header("Value")]
    [SerializeField] float callDistance;                // Player와의 거리

    [Header("Player Reference")]
    [ReadOnlyInspector, SerializeField] Transform target;                  // Player Reference

    public override void Init()
    {
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

            yield return new WaitForSeconds(0.1f);
        }

        animal.ChangeState(AnimalControl.State.CallIdle);
    }
}


/*
    1. 인지
    2. 이모티콘 띄우기
    3. 돌아보기, 다가가기
*/