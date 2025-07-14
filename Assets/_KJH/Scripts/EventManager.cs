using UnityEngine;
using UnityEngine.Events;
public class EventManager : BehaviourSingleton<EventManager>
{
    protected override bool IsDontDestroy() => false;
    public UnityAction<AnimalControl.State> ChangeStateAction = (x) => { };
    public UnityAction<AnimalControl.Effect> OnUpdateEffect = (x) => { };
    











}
