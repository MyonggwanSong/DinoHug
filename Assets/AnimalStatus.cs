using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStatus : MonoBehaviour
{
    public AnimalControl animalControl;
    public float currHP;
    public float maxHP;
    public float hungry;
    void Update()
    {
        hungry -= 0.01f * Time.deltaTime;
        if (hungry <= 0)
        {
            currHP -= 0.01f * Time.deltaTime;
        }
        if (currHP <= 0)
        {
            animalControl.state = AnimalControl.State.Dead;
        }
    }
}
