using System.Collections;
using UnityEngine;
public class AnimalHug : AnimalAbility
{
    [HideInInspector] public bool isPlaying = false;
    [ReadOnlyInspector] public Transform leftControllerTr;
    [ReadOnlyInspector] public Transform rightControllerTr;
    // float huggingStateChangeDelay = 1f; // 1초 딜레이
    // float lastHuggingStateChangeTime = 0f;
    // // isHugging이 true가 된 후 유예시간
    // float huggingGracePeriod = 1f; // 1초 유예시간
    // float huggingStartTime = 0f;
    public override void Init()
    {
        StartCoroutine(nameof(StartLoop));
        animal.HeadIK_OFF();
        coSuccess = null;
        animal.petStateController.UpdateIsInteraction(false);
    }
    public override void UnInit()
    {
        base.UnInit();
        StopCoroutine(nameof(StartLoop));
        animal.HeadIK_ON();
    }
    IEnumerator StartLoop()
    {
        while (true)
        {
            yield return null;

            // 실패조건.. 
            // 앙손의 거리가 좌우, 상하로 너무 벌어질시 or 좌우로 너무 좁아질시 손에서 공룡 놓치는 처리
            float gapHorz = new Vector3(leftControllerTr.position.x - rightControllerTr.position.x, 0f, leftControllerTr.position.z - rightControllerTr.position.z).magnitude;
            float gapVert = Mathf.Abs(leftControllerTr.position.y - rightControllerTr.position.y);
            //Debug.Log($"좌우차이 : {gapHorz} , 상하차이 : {gapVert}");
            if (gapHorz > 0.7f || gapHorz < 0.3f)
            {
                animal.ChangeState(AnimalControl.State.Idle);
                yield break;
            }
            else if (gapVert > 0.25f)
            {
                animal.ChangeState(AnimalControl.State.Idle);
                yield break;
            }

            
            // 안아주기 위치 = 양 컨트롤러 중앙(높이는 조율) + 카메라 전방 0.5m 앞
            Vector3 offset = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized * 0.5f;
            Vector3 hugPos = (leftControllerTr.position + rightControllerTr.position) * 0.5f + offset;
            hugPos.y -= 0.5f;
            transform.LookAt(Camera.main.transform.position);
            transform.position = hugPos;
            if (coSuccess == null)
            {
                coSuccess = StartCoroutine(SuccessAnimation());
            }




        }
    }
    Coroutine coSuccess;
    IEnumerator SuccessAnimation()
    {
        anim.SetInteger("animation", 2); // 행복한 모션
        animal.ChangeFaceTemporal(AnimalControl.Face.Joyful, 1f); // 표정 변화
        AudioManager.Instance.PlayEffect("Happy", transform.position + Vector3.up * 1.2f, 1f); // SFX
        Vector3 _particleOsset = transform.position + new Vector3(0f, 0.5f, 0f);
        ParticleManager.Instance.SpawnParticle(ParticleFlag.Twinkle, _particleOsset, Quaternion.identity, null); // particle
        AudioManager.Instance.PlayEffect("Petting", transform.position + Vector3.up * 1.2f, 1f); // SFX
        yield return new WaitForSeconds(2f);
        anim.SetInteger("animation", 1); // Idle 모션
        animal.ChangeFace(AnimalControl.Face.Default); // 원래 표정으로
        animal.petStateController.Petting();
        animal.petStateController.Petting();
        animal.petStateController.Petting();
        yield return new WaitForSeconds(2f);
        coSuccess = null;
    }

}









// //     // 상태 변경
// //     aHandle.
// AnimalHug aHug;isHugging = shouldHug;
// //     lastHuggingStateChangeTime = Time.time;

// //     // isHugging이 true가 되는 순간 시작 시간 기록
// //     if (shouldHug)
// //     {
// //         if (leftController != null)
// //             leftController.SendHapticImpulse(0.5f, 0.2f);
// //         if (rightController != null)
// //             rightController.SendHapticImpulse(0.5f, 0.2f);

// //         huggingStartTime = Time.time;
// //         Debug.Log("안아주기 시작 - 유예시간 시작");

// //         // 안아주기도 Bond를 오르도록 할 것인가?
// //         // ac.petStateController.Petting();
// //         Vector3 _particleOsset = transform.position + new Vector3(0f, 1.2f, 0f);
// //         ParticleManager.Instance.SpawnParticle(ParticleFlag.Twinkle, _particleOsset, Quaternion.identity, null);
// //     }
// //     else
// //     {
// //         Debug.Log("안아주기 종료");
// //     }
// // }

// //     bool IsValidPettingDirection(Vector3 delta)
// //     {
// //         // 1. 최소 움직임 크기 체크 (너무 작으면 방향 설정 안함)
// //         if (delta.magnitude < 0.01f)
// //         {
// // //          Debug.Log("움직임이 너무 작음");
// //             return false;
// //         }

// //         // 2. 상하 움직임 제한 (Z축 움직임이 30도 이상이면 안됨)
// //         if (Math.Abs(velocity.z) > 0.5f)
// //         {
// // //          Debug.Log("앞뒤 움직임이 너무 큼");
// //             return false;
// //         }

// //         // 3. 좌우 움직임 체크 (X축 또는 Y축 움직임이 주를 이뤄야 함)
// //         float horizontalMovement = Mathf.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);
// //         if (horizontalMovement < 0.7f) // 전체 움직임의 70% 이상이 좌우 움직임이어야 함
// //         {
// // //          Debug.Log($"좌우 움직임 부족: {horizontalMovement:F3}");
// //             return false;
// //         }

// // //      Debug.Log($"유효한 쓰다듬기 방향 - 수평이동: {horizontalMovement:F3}");
// //         return true;
// //     }


