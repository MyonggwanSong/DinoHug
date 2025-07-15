using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HuggingTrigger : MonoBehaviour
{
    public ActionBasedController leftController;
    public ActionBasedController rightController;

    bool con1Clicked;
    bool con2Clicked;


    // 쓰다듬기 판정 변수
    float horizontal;
    bool isFirstMove = true;

    // 컴포넌트 참조
    AnimalControl ac;
    AnimalHug ah;

    // 타이머
    float _elapsed;

    // 쓰다듬기 상태 변화 딜레이
    float pettingStateChangeDelay = 1f; // 0.2초 딜레이
    float lastHuggingStateChangeTime = 0f;

    // isPetting이 true가 된 후 유예시간
    float pettingGracePeriod = 1f; // 1초 유예시간
    float huggingStartTime = 0f;

    void Awake()
    {
        ac = GetComponentInParent<AnimalControl>();
        if (ac == null)
            Debug.Log("HuggingTrigger] AnimalControl 이 없습니다.");

        ah = GetComponentInParent<AnimalHug>();
        if (ah == null)
            Debug.Log("HuggingTrigger] AnimalHug 이 없습니다.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && leftController == null)
        {
            ActionBasedController controller = other.GetComponentInParent<ActionBasedController>();
            if (controller.gameObject.name == "Left Controller")
            {
                leftController = controller;
                leftController.SendHapticImpulse(0.5f, 0.2f);
            }
            else
            {
                rightController = controller;
                rightController.SendHapticImpulse(0.5f, 0.2f);
            }
        }


    }
    Vector3 con1Position;
    Vector3 con2Position;
    void OnTriggerStay(Collider other)
    {
        if ((leftController == null && rightController == null) || (leftController.activateAction.action == null && rightController.activateAction.action == null))
            return;
        float input1 = -1f;
        float input2 = -1f;

        if (leftController != null)
            input1 = leftController.activateAction.action.ReadValue<float>();

        if (rightController != null)
            input2 = rightController.activateAction.action.ReadValue<float>();



        Debug.Log($"leftController Input: {input1}");
        Debug.Log($"rightController Input: {input2}");


        if (input1 == 1f) // 버튼이 눌린 상태
        {
            // 1. 

            con1Clicked = true;
            if (ac.state != AnimalControl.State.Handle) // AnimalControl.State.Hug 로 변경
            {
                ac.ChangeState(AnimalControl.State.Handle);
                //Debug.Log("상태 변경: Handle");
            }

            // 2. 컨트롤러 위치 업데이트
            con1Position = leftController.transform.position;

            // // 3. 쓰다듬기 판정
            // CheckPettingMotion();

            // // 4. 타이머 리셋
            // _elapsed = 0f;
        }
        else // 버튼이 안 눌린 상태
        {
            // 즉시 쓰다듬기 중단 (딜레이 적용)
            UpdatePettingState(false);

            // 타이머 증가
            _elapsed += Time.deltaTime;
            if (_elapsed > 0.5f)
            {
                ResetToIdle(false, leftController);
            }
        }

        if (input2 == 1f) // 버튼이 눌린 상태
        {
            // 1. 버튼 클릭시 햅틱반응
           
            con2Clicked = true;
            if (ac.state != AnimalControl.State.Handle) // AnimalControl.State.Hug 로 변경
            {
                ac.ChangeState(AnimalControl.State.Handle);
                //Debug.Log("상태 변경: Handle");
            }

            // 2. 컨트롤러 위치 업데이트
            con2Position = rightController.transform.position;

            // // 3. 쓰다듬기 판정
            // CheckPettingMotion();

            // // 4. 타이머 리셋
            // _elapsed = 0f;
        }
        else // 버튼이 안 눌린 상태
        {
            // 즉시 쓰다듬기 중단 (딜레이 적용)
            UpdatePettingState(false);


        }

        if (con1Clicked && con2Clicked)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed > 0.5f)
            {
                ah.isHugging = true;
                leftController.SendHapticImpulse(0.5f, 0.2f);
                rightController.SendHapticImpulse(0.5f, 0.2f);

                Vector3 hugPos = (con1Position + con2Position) * 0.5f + Camera.main.transform.forward.normalized * 0.75f;

                ac.transform.position = hugPos;

                Debug.Log("안아주기 시작 - 유예시간 시작");
                ac.petStateController.Petting();            // Hugging 으로 바꿔야함
                ResetToIdle(false, leftController);
                ResetToIdle(false, rightController);

            }

        }
    }

    // #region _PWH        
    // Vector3 center = new Vector3(0, 0, 0);

    // void Update()
    // {
    //     if (ap.isPetting)
    //     {
    //         UpdateParticlePosition();
    //     }
    // }

    // float elapsed = 0f;
    // [SerializeField] float frequency = 0.2f;

    // void UpdateParticlePosition()
    // {
    //     if (controller == null) return;

    //     elapsed += Time.deltaTime;

    //     if (elapsed >= frequency)
    //     {
    //         elapsed = 0;
    //         center = controller.transform.position;
    //         ParticleManager.Instance.SpawnParticle(ParticleFlag.Petting, center, Quaternion.identity, this.gameObject.transform);
    //     }
    // }

    // #endregion


    void UpdatePettingState(bool shouldHug)
    {
        // 현재 상태와 원하는 상태가 같으면 아무것도 하지 않음
        if (ah.isHugging == shouldHug)
            return;

        // 마지막 상태 변경으로부터 충분한 시간이 지났는지 확인
        if (Time.time - lastHuggingStateChangeTime < pettingStateChangeDelay)
            return;

        // isPetting이 true에서 false로 바뀌는 경우 유예시간 체크
        if (ah.isHugging == true && shouldHug == false)
        {
            // 유예시간이 아직 지나지 않았으면 false로 바꾸지 않음
            if (Time.time - huggingStartTime < pettingGracePeriod)
            {
                //Debug.Log($"유예시간 중 - 남은시간: {pettingGracePeriod - (Time.time - huggingStartTime):F1}초");
                return;
            }
        }

        // 상태 변경
        ah.isHugging = shouldHug;

        lastHuggingStateChangeTime = Time.time;

        // isPetting이 true가 되는 순간 시작 시간 기록
        if (shouldHug)
        {
            //rightController.SendHapticImpulse(0.5f, 0.2f);
            huggingStartTime = Time.time;
            //Debug.Log("쓰다듬기 시작 - 유예시간 시작");
            //ac.petStateController.Petting();
        }

        //Debug.Log($"쓰다듬기 상태 변경: {shouldHug}");
    }
    void OnTriggerExit(Collider other)
    {
        Action action = other.GetComponentInParent<ActionBasedController>() == leftController
        ? (() => ResetToIdle(true, leftController))
        : (() => ResetToIdle(true, rightController));
    }

    void ResetToIdle(bool disconnect, ActionBasedController con)
    {

        // 데이터 초기화
        if (con == leftController)
        {
            con1Position = Vector3.zero;
            con1Clicked = false;

            if (disconnect)
                leftController = null;

        }
        else
        {
            con2Position = Vector3.zero;
            con2Clicked = false;

            if (disconnect)
                rightController = null;

        }


        // 상태 초기화

        UpdatePettingState(false); // 딜레이 적용해서 false로 설정

        if (ac.state != AnimalControl.State.Play)
        {
            ac.ChangeState(AnimalControl.State.Idle);
        }


        // 타이머 리셋
        _elapsed = 0f;
        lastHuggingStateChangeTime = 0f; // 딜레이 타이머 리셋
        huggingStartTime = 0f; // 유예시간 타이머 리셋



    }
}
