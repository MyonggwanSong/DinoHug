using UnityEngine;
public abstract class AnimalAbility : MonoBehaviour
{
    public abstract void Init();
    public abstract void UnInit();
    protected AnimalControl.State state;
    protected AnimalControl.Effect effect;
    protected Animator anim;
}
