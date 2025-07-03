using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimalHandle : AnimalAbility
{
    public override void Init()
    {

    }
    public override void UnInit()
    {

    }
    IEnumerator Play()
    {
        yield return null;

        anim.Play("Handle");

        
    }
    
}
