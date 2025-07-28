using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class HandlingingTrigger : MonoBehaviour
{
    private ActionBasedController leftController;
    private ActionBasedController rightController;
    AnimalControl ac;
    AnimalHandle aHandle;
    AnimalHug aHug;
    void Awake()
    {
        ac = GetComponentInParent<AnimalControl>();
        if (ac == null)
            Debug.Log("PettingTrigger] AnimalControl 이 없습니다.");
        aHandle = GetComponentInParent<AnimalHandle>();
        if (aHandle == null)
            Debug.Log("PettingTrigger] AnimalHandle 이 없습니다.");
        aHug = GetComponentInParent<AnimalHug>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("GameController")) return;
        ActionBasedController controller;
        if (leftController == null)
        {
            controller = other.GetComponentInParent<ActionBasedController>();
            if (controller != null && controller.gameObject.name == "Left Controller")
            {
                leftController = controller;
                leftController.SendHapticImpulse(0.5f, 0.2f);
            }
        }
        if (rightController == null)
        {
            controller = other.GetComponentInParent<ActionBasedController>();
            if (controller != null && controller.gameObject.name == "Right Controller")
            {
                rightController = controller;
                rightController.SendHapticImpulse(0.5f, 0.2f);
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("GameController")) return;
        if (input1 == 1f && input2 == 1f)
        {
            if (ac.state != AnimalControl.State.Hug)
            {
                aHug.leftControllerTr = leftController.transform;
                aHug.rightControllerTr = rightController.transform;
                ac.ChangeState(AnimalControl.State.Hug);
            }
        }
        else if(input1 == 1f || input2 == 1f)
        {
            if (ac.state != AnimalControl.State.Handle)
            {
                if (leftController != null)
                    aHandle.controllerTr = leftController.transform;
                else
                    aHandle.controllerTr = rightController.transform;
                ac.ChangeState(AnimalControl.State.Handle);
            }
        }
    }
    float input1 = -1f;
    float input2 = -1f;
    void Update()
    {
        if (leftController != null)
            input1 = leftController.activateAction.action.ReadValue<float>();
        if (rightController != null)
            input2 = rightController.activateAction.action.ReadValue<float>();
        if (input1 < 1f && input2 < 1f)
        {
            if (ac.state == AnimalControl.State.Hug)
                ac.ChangeState(AnimalControl.State.Idle);
            if (ac.state == AnimalControl.State.Handle)
                ac.ChangeState(AnimalControl.State.Idle);
        }
        else if (input1 < 1f || input2 < 1f)
        {
            if (ac.state == AnimalControl.State.Hug)
                ac.ChangeState(AnimalControl.State.Idle);
        }
    }

}




