using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Food : MonoBehaviour
{
    public bool isPlace;
    bool isGrab;
    void OnCollisionStay(Collision collision)
    {
        if (isPlace) return;
        if (isGrab) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isPlace = true;
        }
    }
    public void Grab()
    {
        isGrab = true;
        isPlace = false;
    }
    public void UnGrab()
    {
        isGrab = false;
    }



}
