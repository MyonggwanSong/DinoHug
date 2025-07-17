using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



public class AnimalPet : AnimalAbility
{

    [ReadOnlyInspector] public ActionBasedController controller;
    public bool isPetting = false;
    public bool isHugging = false;




    Coroutine pet_co = null;
    Coroutine hug_co = null;


    public override void Init()
    {
        pet_co = StartCoroutine(Pet());
        hug_co = StartCoroutine(Hug());

    }

    public override void UnInit()
    {
        base.UnInit();
        StopCoroutine(pet_co);
        StopCoroutine(hug_co);

    }

    IEnumerator Pet()
    {
        yield return new WaitUntil(() => isPetting && !isHugging);
        anim.SetInteger("animation", 2); // 행복한 모션
                                         //Debug.Log("쓰다듬는 중");

        yield return new WaitUntil(() => !isPetting);
        anim.SetInteger("animation", 1); // Idle 모션
        //Debug.Log("쓰다듬기 끝");
    }
    
     IEnumerator Hug()  
    {
        yield return new WaitUntil(() => isHugging && !isPetting);
        transform.LookAt(Camera.main.transform.position);
        anim.SetInteger("animation", 27); // 귀여운 모션
                                         //Debug.Log("쓰다듬는 중");

        yield return new WaitUntil(() => !isHugging);
        anim.SetInteger("animation", 1); // Idle 모션
        //Debug.Log("쓰다듬기 끝");
    }

}
