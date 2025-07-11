using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Toy : MonoBehaviour
{
    public AnimalControl animal;
    public XRGrabInteractable xRGrab;
    public Throw t;

    public bool isPlaced;
    public bool isGrabbed;
    public bool isThrow;

    public void GrabStart()
    {
        isGrabbed = true;
        isPlaced = false;
        isThrow = false;
        animal.ChangeState(AnimalControl.State.Play);
    }
    public void GrabEnd()
    {
        isGrabbed = false;
    }

    private IEnumerator Retry()
    {
        while(true)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
            yield return new WaitUntil(() => animal.state == AnimalControl.State.Idle || animal.state == AnimalControl.State.Wander);
            animal.ChangeState(AnimalControl.State.Play);
        }
    }
        
    public void OnDisableGrab()
    {
        xRGrab.enabled = false;
    }

}
