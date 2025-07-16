using System.Collections;
using DG.Tweening;
using UnityEngine;

public class AnimalCallFollow : AnimalAbility
{
    [Header("Value")]
    [SerializeField] float callDistance;                                    // Player와의 거리

    [Header("Player Reference")]
    [ReadOnlyInspector, SerializeField] Transform target;                   // Player Reference

    [Header("Icon")]
    [SerializeField] GameObject icon;

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
        
        DOVirtual.DelayedCall(0.0f, () => ShowIcon());
        DOVirtual.DelayedCall(0.05f, () => MakeSound());
        
        yield return new WaitForSeconds(0.3f);

        //돌아보기
        Vector3 targetPosition = target.position;
        targetPosition.y = 0f;
        transform.DOLookAt(targetPosition, 0.4f);

        yield return new WaitForSeconds(0.42f);
        anim.SetInteger("animation", 18);

        // 거리가 좁혀 질 때까지 다가가기
        while (Vector3.Distance(gameObject.transform.position, target.transform.position) > callDistance)
        {
            bool check = agent.SetDestination(target.transform.position + (Vector3.forward * 0.5f));

            if (!check)
            {
                Debug.Log("목적지 찾기 실패...");
            }
            yield return null;
        }

        animal.ChangeState(AnimalControl.State.CallIdle);
        yield break;
    }

    void ShowIcon()
    {
        icon.SetActive(true);
    }

    void MakeSound()
    {
        SFX sfx = AudioManager.Instance.PlayEffect("Crying", this.gameObject.transform.position);
    }
}