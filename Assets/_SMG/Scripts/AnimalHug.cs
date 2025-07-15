using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



public class AnimalHug : AnimalAbility
{

    [ReadOnlyInspector] public ActionBasedController controller;
    public bool isHugging = false;

    
   

    Coroutine _co = null;

    public override void Init()
    {
        _co = StartCoroutine(Hug());
    }

    public override void UnInit()
    {
        base.UnInit();
        StopCoroutine(_co);
    }

    IEnumerator Hug()
    {
        yield return new WaitUntil(() => isHugging);
        transform.LookAt(Camera.main.transform.position);
        
        anim.SetInteger("animation", 2); // 행복한 모션
                                         //Debug.Log("쓰다듬는 중");

        yield return new WaitUntil(() => !isHugging);
        anim.SetInteger("animation", 1); // Idle 모션
        //Debug.Log("쓰다듬기 끝");

        // yield return null;
        // if (isPetting)
        // {
        //     anim.CrossFade("aniClip1", 0.2f);
        //     Debug.Log("쓰다듬는 중");
        // }
        // else
        // {
        //     anim.CrossFade("Idle", 0.2f);
        //        Debug.Log("쓰다듬기 끝");
        // }


    }

}
