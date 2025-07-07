using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



public class AnimalPet : AnimalAbility
{  
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
        yield return null;

        

        anim.Play("aniClip1");



        Debug.Log("쓰다듬는 중");


    }
  
}
