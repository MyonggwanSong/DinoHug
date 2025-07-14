using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimalCall : AnimalAbility
{
    [Header("Value")]
    [SerializeField] float callDistance;                // Player와의 거리
    [SerializeField] float callWaitTime;                // Player에게 도착 후 대기시간.

    [Header("Player Reference")]
    [SerializeField] Transform target;                  // Player Reference

    public override void Init()
    {
        // 상태 시작
        StartCoroutine(nameof(CallSequence));
    }

    public override void UnInit()
    {
        base.UnInit();
        StopCoroutine(nameof(CallSequence));
        agent.isStopped = true;
    }

    // 동물이 호출을 받았다면. 실행할 것들
    IEnumerator CallSequence()
    {
        yield return null;
        Debug.Log("[Animal Call] : Player 호출 인지");
        yield return new WaitForSeconds(0.5f);
        
    }
}


/*
    1. 인지
    2. 이모티콘 띄우기
    3. 돌아보기, 다가가기
*/