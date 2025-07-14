using System.Collections;
using UnityEngine;
public class AnimalDead : AnimalAbility
{
    public override void Init()
    {

    }
    public override void UnInit()
    {
        base.UnInit();

    }
    IEnumerator Die()
    {
        anim.CrossFade("", 0.2f);

        yield return YieldInstructionCache.WaitForSeconds(2f);



        yield return YieldInstructionCache.WaitForSeconds(2f);

        animal.ChangeState(AnimalControl.State.Idle);
    }

    
    
}
