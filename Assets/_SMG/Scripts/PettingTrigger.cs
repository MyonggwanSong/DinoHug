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
        if (other.CompareTag("GameController") && controller == null)
        {
            controller = other.GetComponentInParent<ActionBasedController>();

            currPosition = controller.transform.position;

        }
    }
    bool isStable = false;
    void OnTriggerStay(Collider other)
    {
        if (controller == null || controller.activateAction == null || controller.activateAction.action == null)
            return;
        float input = controller.activateAction.action.ReadValue<float>();
        Debug.Log(input);

        if (input == 1)
        {
            if (!isStable)                                          // 클릭시 멈춰있으면 handle로 상태 변경
            {
                isStable = true;
                ac.ChangeState(AnimalControl.State.Handle);
            }

            if (currPosition != Vector3.zero)                       // 현재 컨트롤러의 위치값이 있으면 이전 위치에 넣음
                prevPosition = currPosition;

            currPosition = controller.transform.position;           // 현재 컨트롤러의 위치값 가져옴
            velocity = (currPosition - prevPosition).normalized;    // 컨트롤러의 방향벡터 구하기
            if (firstDirection == Vector3.zero && Math.Abs(velocity.y) < 0.5)                     // 컨트롤러 초기 방향벡터가 없고, x,z평면과 y축이 30도 미만일 때만 해당 방향벡터를 방향벡터로 넣어줌
            {
              
                    firstDirection = velocity;
            }
            horizontal = Vector3.Dot(firstDirection, velocity);     // 초기 방향벡터와 현재 방향벡터 내적



            // Debug.Log($"dot product : {horizontal.ToString("F3")}");   // Dot product 디버그

            float speed = (currPosition - prevPosition).magnitude / Time.deltaTime;
            Debug.Log($"Shake Speed : {speed.ToString("F3")}"); // Velocity 디버그

            if ((horizontal > 0.7f || horizontal < -0.7f) && ac.state == AnimalControl.State.Handle && speed > 200f)
            {
                if (delay_co != null)
                    StopCoroutine(delay_co);
                ap.isPetting = true;

            }
            else
            {
                delay_co = StartCoroutine(DelayResetData(false));
                //ResetData(false);
            }
        }
        else
        {
            // ResetData(false);
            delay_co = StartCoroutine(DelayResetData(false));
        }
    }

    void OnTriggerExit(Collider other)          
    {
        //ResetData(true);
        delay_co = StartCoroutine(DelayResetData(true));
    }



    IEnumerator DelayResetData(bool disconnect)     // 2초동안 행동 안할 시 값 초기화
    {
        yield return new WaitForSeconds(2f);

        prevPosition = Vector3.zero;
        currPosition = Vector3.zero;
        firstDirection = Vector3.zero;
        isStable = false;
        ap.isPetting = false;
        if (disconnect)
        {
            controller = null;
        }
    }
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
