using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimalCallIdle : AnimalAbility
{
    [SerializeField] float callWaitTime;                // Player에게 도착 후 대기시간. => 대기 시간을 넘어가면 다시 wandor 상태로 넘어가기

    public override void Init()
    {
        StopCoroutine(nameof(StartCallIdle));
        StartCoroutine(nameof(StartCallIdle));
    }

    public override void UnInit()
    {
        base.UnInit();
        Debug.Log($"UnInit CallIdle.");
        StopCoroutine(nameof(StartCallIdle));
    }

    IEnumerator StartCallIdle()
    {
        Debug.Log("Target에 도착했습니다.");
        LerpAngle();

        agent.isStopped = true;

        // 도착한 상태 멈추기
        yield return new WaitForSeconds(callWaitTime);

        //이 시간 이후에도 동물의 상태가 Call이라면. Idle 상태로 넘어가기
        if (animal.state.Equals(AnimalControl.State.CallIdle))
        {
            Debug.Log("시간 초과... Idle 상태로 이동합니다.");
            animal.ChangeState(AnimalControl.State.Idle);
        }
    }

    void LerpAngle()
    {
        Transform target = Camera.main.transform;

        Vector3 targetPosition = target.position;
        targetPosition.y = 0f;

        transform.DOLookAt(targetPosition, 0.3f);
    }
}
