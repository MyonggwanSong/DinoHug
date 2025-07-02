using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimalControl : MonoBehaviour
{
    public enum State
    {
        Idle,
        //Wander,
        GoTo,
        Eat,
    }
    public enum Effect
    {
        None,
        Hungry,
    }
    public State state;
    public Effect effect;
}
