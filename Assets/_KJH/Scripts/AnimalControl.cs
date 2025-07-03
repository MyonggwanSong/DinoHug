using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimalControl : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(nameof(StateMachine));
    }
    public enum State
    {
        Idle,
        //Wander,
        GoTo,
        Eat,
        Handle,
        Drink,
        Play,
        Dead,


    }
    public enum Effect
    {
        None,
        Hungry,
        Thirsty,
        Bored,
    }
    public State state;
    public Effect effect;
    IEnumerator StateMachine()
    {
        while (true)
        {
            yield return null;



            // if (!isPlaying.ContainsKey(state.Name)) continue;
            // else if (!isPlaying[state.Name])
            // {
            //     if (prevCo != null) StopCoroutine(prevCo);
            //     if (prevStateName != null) isPlaying[prevStateName] = false;
            //     isPlaying[state.Name] = true;
            //     //Debug.Log($"{state.Name}라는 동작 하는 중");
            //     prevCo = StartCoroutine(state.Name);
            //     prevStateName = state.Name;
            // }
        }
    }



}
