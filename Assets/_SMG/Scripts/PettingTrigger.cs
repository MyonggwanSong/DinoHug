using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PettingTrigger : MonoBehaviour
{
    public ActionBasedController controller;

    // 위치 추적 변수
    Vector3 prevPosition;
    Vector3 currPosition;
    Vector3 velocity;
    Vector3 firstDirection = Vector3.zero;

    // 쓰다듬기 판정 변수
    float horizontal;
    bool isFirstMove = true;

    // 컴포넌트 참조
    AnimalControl ac;
    AnimalPet ap;

    // 타이머
    float _elapsed;

    // 쓰다듬기 상태 변화 딜레이
    float pettingStateChangeDelay = 0.2f; // 0.2초 딜레이
    float lastPettingStateChangeTime = 0f;

    // isPetting이 true가 된 후 유예시간
    float pettingGracePeriod = 1f; // 1초 유예시간
    float pettingStartTime = 0f;

    void Awake()
    {
        ac = GetComponentInParent<AnimalControl>();
        if (ac == null)
            Debug.Log("PettingTrigger] AnimalControl 이 없습니다.");

        ap = GetComponentInParent<AnimalPet>();
        if (ap == null)
            Debug.Log("PettingTrigger] AnimalPet 이 없습니다.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController") && controller == null)
        {
            controller = other.GetComponentInParent<ActionBasedController>();
            controller.SendHapticImpulse(0.5f, 0.2f);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (controller == null || controller.activateAction.action == null)
            return;

        float input = controller.activateAction.action.ReadValue<float>();
        //Debug.Log($"Input: {input}");

        if (input == 1f) // 버튼이 눌린 상태
        {
            // 1. Handle 상태로 전환
            if (ac.state != AnimalControl.State.Handle)
            {
                ac.ChangeState(AnimalControl.State.Handle);
                //Debug.Log("상태 변경: Handle");
            }

            // 2. 위치 추적 및 속도 계산
            UpdatePosition();

            // 3. 쓰다듬기 판정
            CheckPettingMotion();

            // 4. 타이머 리셋
            _elapsed = 0f;
        }
        else // 버튼이 안 눌린 상태
        {
            // 즉시 쓰다듬기 중단 (딜레이 적용)
            UpdatePettingState(false);

            // 타이머 증가
            _elapsed += Time.deltaTime;
            if (_elapsed > 0.5f)
            {
                ResetToIdle(false);
            }
        }
    }

    void UpdatePettingState(bool shouldPet)
    {
        // 현재 상태와 원하는 상태가 같으면 아무것도 하지 않음
        if (ap.isPetting == shouldPet)
            return;

        // 마지막 상태 변경으로부터 충분한 시간이 지났는지 확인
        if (Time.time - lastPettingStateChangeTime < pettingStateChangeDelay)
            return;

        // isPetting이 true에서 false로 바뀌는 경우 유예시간 체크
        if (ap.isPetting == true && shouldPet == false)
        {
            // 유예시간이 아직 지나지 않았으면 false로 바꾸지 않음
            if (Time.time - pettingStartTime < pettingGracePeriod)
            {
                //Debug.Log($"유예시간 중 - 남은시간: {pettingGracePeriod - (Time.time - pettingStartTime):F1}초");
                return;
            }
        }

        // 상태 변경
        ap.isPetting = shouldPet;

        lastPettingStateChangeTime = Time.time;

        // isPetting이 true가 되는 순간 시작 시간 기록
        if (shouldPet)
        {
            controller.SendHapticImpulse(0.5f, 0.2f);
            pettingStartTime = Time.time;
            Debug.Log("쓰다듬기 시작 - 유예시간 시작");
            ac.petStateController.Petting();
        }

        //Debug.Log($"쓰다듬기 상태 변경: {shouldPet}");
    }

    bool IsValidPettingDirection(Vector3 delta)
    {
        // 1. 최소 움직임 크기 체크 (너무 작으면 방향 설정 안함)
        if (delta.magnitude < 0.01f)
        {
            //Debug.Log("움직임이 너무 작음");
            return false;
        }

        // 2. 상하 움직임 제한 (Y축 움직임이 30도 이상이면 안됨)
        if (Math.Abs(velocity.y) > 0.5f)
        {
            //Debug.Log("상하 움직임이 너무 큼");
            return false;
        }

        // 3. 좌우 움직임 체크 (X축 또는 Z축 움직임이 주를 이뤄야 함)
        float horizontalMovement = Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z);
        if (horizontalMovement < 0.7f) // 전체 움직임의 70% 이상이 좌우 움직임이어야 함
        {
            //Debug.Log($"좌우 움직임 부족: {horizontalMovement:F3}");
            return false;
        }

        //Debug.Log($"유효한 쓰다듬기 방향 - 수평이동: {horizontalMovement:F3}");
        return true;
    }

    void UpdatePosition()
    {
        // 이전 위치 저장
        if (currPosition != Vector3.zero)
            prevPosition = currPosition;

        // 현재 위치 업데이트
        currPosition = controller.transform.position;

        // 첫 번째 프레임은 delta 계산 불가
        if (prevPosition == Vector3.zero)
            return;

        Vector3 delta = currPosition - prevPosition;

        // 움직임이 너무 작으면 쓰다듬기 중단
        if (delta.sqrMagnitude < 0.001f)
        {
            UpdatePettingState(false);
            return;
        }

        // 속도 벡터 계산
        velocity = delta.normalized;
    }

    void CheckPettingMotion()
    {
        // 위치 데이터가 충분하지 않으면 리턴
        if (prevPosition == Vector3.zero || currPosition == Vector3.zero)
            return;

        Vector3 delta = currPosition - prevPosition;

        // 상하 움직임이 너무 크면 쓰다듬기 불가
        if (Math.Abs(velocity.y) > 0.5f)
        {
            UpdatePettingState(false);
            //Debug.Log("상하 움직임 감지 - 쓰다듬기 중단");
            return;
        }

        // 첫 번째 유효한 움직임 방향 저장 (좌우 방향만 허용)
        if (firstDirection == Vector3.zero && isFirstMove)
        {
            if (IsValidPettingDirection(delta))
            {
                firstDirection = velocity;
                isFirstMove = false;
                //Debug.Log($"첫 번째 방향 설정: {firstDirection}");
            }
            else
            {
                //Debug.Log("유효하지 않은 방향 - firstDirection 설정 안함");
            }
        }

        // 첫 번째 방향이 설정되지 않았으면 리턴
        if (firstDirection == Vector3.zero)
            return;

        // 현재 움직임과 첫 번째 방향의 내적 계산
        horizontal = Vector3.Dot(firstDirection, velocity);

        // 쓰다듬기 조건 체크
        bool shouldPet = Math.Abs(horizontal) > 0.7f &&
                        ac.state == AnimalControl.State.Handle;

        // 쓰다듬기 상태 업데이트 (딜레이 적용)
        UpdatePettingState(shouldPet);

        //Debug.Log($" Horizontal: {horizontal:F3}, IsPetting: {ap.isPetting}, ShouldPet: {shouldPet}");
    }

    void OnTriggerExit(Collider other)
    {
        ResetToIdle(true);
    }

    void ResetToIdle(bool disconnect)
    {
        //Debug.Log("Idle 상태로 리셋");

        // 위치 데이터 초기화
        prevPosition = Vector3.zero;
        currPosition = Vector3.zero;
        firstDirection = Vector3.zero;

        // 상태 초기화
        isFirstMove = true;
        UpdatePettingState(false); // 딜레이 적용해서 false로 설정

        if(ac.state != AnimalControl.State.Play)
        {
            ac.ChangeState(AnimalControl.State.Idle);
        }
        

        // 타이머 리셋
        _elapsed = 0f;
        lastPettingStateChangeTime = 0f; // 딜레이 타이머 리셋
        pettingStartTime = 0f; // 유예시간 타이머 리셋

        // 컨트롤러 연결 해제
        if (disconnect)
            controller = null;
    }
}