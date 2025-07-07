using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimalIdle : AnimalAbility
{
    public override void Init()
    {
        Debug.Log("공룡 Idle 시작");
        StartCoroutine(Idle());
    }
    public override void UnInit()
    {
        //Debug.Log("공룡 Idle 끝");
    }
    IEnumerator Idle()
    {
        anim.CrossFade("Idle", 0.1f);
        yield return new WaitForSeconds(3f);


        yield return new WaitForSeconds(Random.Range(0f, 2f));
        // Idle이 시간이 다 되어서 정상적인 종료일시
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
