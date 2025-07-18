using UnityEngine;
using UnityEngine.AI;
public abstract class AnimalAbility : MonoBehaviour
{
    public abstract void Init();
    public virtual void UnInit()
    {
        agent.ResetPath();
        StopAllCoroutines();
        animal.petStateController.UpdateIsInteraction(false);
    }
    protected AnimalControl animal;
    protected Animator anim;
    protected NavMeshAgent agent;
    protected virtual void Awake()
    {
        TryGetComponent(out animal);
        TryGetComponent(out agent);
        anim = GetComponentInChildren<Animator>();
    }
    protected virtual void Start(){}
}
