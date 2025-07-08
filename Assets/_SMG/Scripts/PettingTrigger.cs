using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class PettingTrigger : MonoBehaviour
{
    public ActionBasedController controller;
    Vector3 prevPosition;
    Vector3 currPosition;
    Vector3 velocity;
    Vector3 firstDirection = Vector3.zero;
    float horizontal;

    AnimalControl ac;
    AnimalPet ap;
    Coroutine delay_co = null;
    void Awake()
    {
        ac = GetComponentInParent<AnimalControl>();
        ap = GetComponentInParent<AnimalPet>();

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            controller = other.GetComponentInParent<ActionBasedController>();

            currPosition = controller.transform.position;

        }
    }
    bool isStable = false;
    void OnTriggerStay(Collider other)
    {
        float input = controller.activateAction.action.ReadValue<float>();
        Debug.Log(input);

        if (input == 1)
        {
            if (!isStable)
            {
                isStable = true;
                ac.ChangeState(AnimalControl.State.Handle);
            }

            // if (delay_co != null)
            //     StopCoroutine(delay_co);


            if (currPosition != Vector3.zero)
                prevPosition = currPosition;

            currPosition = controller.transform.position;
            velocity = (currPosition - prevPosition).normalized;
            if (firstDirection == Vector3.zero)
            {
                firstDirection = velocity;
            }
            horizontal = Vector3.Dot(firstDirection, velocity);

            //(Math.Abs(firstDirection.y) < 1 && input == 1)


            Debug.Log(horizontal.ToString("F3"));
            if (horizontal > 0.7f || horizontal < -0.7f && ac.state == AnimalControl.State.Handle)
            {
                ap.isPetting = true;
            }
            else
            {
                ap.isPetting = false;

                // delay_co = StartCoroutine(DelayResetData(false));
                ResetData(false);
            }
        }
        else
        ResetData(false);
            //delay_co = StartCoroutine(DelayResetData(false));
    }

    void OnTriggerExit(Collider other)
    {
        ResetData(true);        
    }



    // IEnumerator DelayResetData(bool disconnect)
    // {
    //     yield return new WaitForSeconds(2f);

    //     prevPosition = Vector3.zero;
    //     currPosition = Vector3.zero;
    //     firstDirection = Vector3.zero;
    //     isStable = false;
    //     if (disconnect)
    //     {
    //         controller = null;
    //     }
    // }
    void ResetData(bool disconnect)
    {
        prevPosition = Vector3.zero;
        currPosition = Vector3.zero;
        firstDirection = Vector3.zero;
        isStable = false;
        if (disconnect)
        {
            controller = null;
        }
    }
  


}
