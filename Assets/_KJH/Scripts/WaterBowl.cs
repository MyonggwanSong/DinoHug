using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class WaterBowl : MonoBehaviour
{
    public AnimalControl animalControl;
    public bool isPlaced;
    public bool isFilled;
    XRGrabInteractable xRGrab;
    bool isGrabbed;
    Vector3 startPosition;
    Quaternion startRotation;
    void Awake()
    {
        TryGetComponent(out xRGrab);
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    public void OnGrabStart()
    {
        isGrabbed = true;
        isPlaced = false;
    }
    public void OnGrabEnd()
    {
        isGrabbed = false;
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if (isPlaced) return;
            if (!isGrabbed)
            {
                isPlaced = true;
            }
        }
    }
    public void DisableGrab()
    {
        xRGrab.enabled = false;
    }
    public void EnableGrab()
    {
        xRGrab.enabled = true;
    }
    public void Reset()
    {
        EnableGrab();
        transform.position = startPosition;
        transform.rotation = startRotation;
        isPlaced = false;
        isGrabbed = false;
        isFilled = false;
    }
    public void Fill()
    {
        
    }
    





    


}
