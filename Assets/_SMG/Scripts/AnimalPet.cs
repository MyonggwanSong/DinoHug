using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



public class AnimalPet : AnimalAbility
{

    public ActionBasedController controller;
    public bool isPetting = false;

    
   

    Coroutine _co = null;

    public override void Init()
    {
       _co = StartCoroutine( Pet());
    }

    public override void UnInit()
    {
        StopCoroutine(_co);
    }

    IEnumerator Pet()
    {
        yield return new WaitUntil(() => isPetting);
        anim.CrossFade("aniClip1", 0.2f);
        //Debug.Log("쓰다듬는 중");
        
        yield return new WaitUntil(() => !isPetting);
        anim.CrossFade("Idle", 0.2f);
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
