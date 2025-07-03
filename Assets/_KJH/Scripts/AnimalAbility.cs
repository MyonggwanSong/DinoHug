using System.Collections.Generic;
using UnityEngine;
public abstract class AnimalAbility : MonoBehaviour
{
    public abstract void Init();
    public abstract void UnInit();
    protected AnimalControl animal;
    protected Animator anim;
    protected Dictionary<string, bool> isPlaying = new Dictionary<string, bool>();
    protected virtual void Awake()
    {
        TryGetComponent(out animal);
        anim = GetComponentInChildren<Animator>();
        if (anim == null) Debug.Log("Animal에 Animator 가 없습니다.");
    }
    protected virtual void Start(){}
    

}
