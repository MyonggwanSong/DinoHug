using UnityEngine;
public abstract class AnimalAbility : MonoBehaviour
{
    public abstract void Init();
    public virtual void UnInit()
    {
        StopAllCoroutines();
    }
    protected AnimalControl animal;
    protected Animator anim;
    protected virtual void Awake()
    {
        TryGetComponent(out animal);
        anim = GetComponentInChildren<Animator>();
    }
    protected virtual void Start(){}
}
