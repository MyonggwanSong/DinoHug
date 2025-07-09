using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Toy : MonoBehaviour
{
        public AnimalControl animal;
        XRGrabInteractable xRGrab;
        public bool isPlaced;
        public bool isGrabbed;
        public bool isThrow;

        public void GrabStart()
        {
            isGrabbed = true;
            isPlaced = false;
             animal.ChangeState(AnimalControl.State.Play);
        }
        public void GrabEnd()
        {
            isGrabbed = false;
        }



}
