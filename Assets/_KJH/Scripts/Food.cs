using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Food : MonoBehaviour
{
    AnimalControl animalControl;
    XRGrabInteractable xRGrab;
    public bool isPlaced;
    bool isGrabbed;
    Vector3 startPosition;
    Quaternion startRotation;
    void Awake()
    {
        TryGetComponent(out xRGrab);
        animalControl = FindAnyObjectByType<AnimalControl>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    public void OnGrabStart()
    {
        isGrabbed = true;
        isPlaced = false;
        StopCoroutine(nameof(Retry));
    }
    public void OnGrabEnd()
    {
        isGrabbed = false;
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
        StopCoroutine(nameof(Retry));
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if (isPlaced) return;
            if (!isGrabbed)
            {
                isPlaced = true;
                // Idle, Wander 상태일때만 --> State.Eat 으로 체인지
                if (animalControl.state == AnimalControl.State.Idle || animalControl.state == AnimalControl.State.Wander)
                {
                    if(animalControl.state != AnimalControl.State.Eat)
                        animalControl.ChangeState(AnimalControl.State.Eat);
                }
                StartCoroutine(nameof(Retry));
            }
        }
    }
    // 푸드가 바닥에 떨어져있고 + Idle, Wander 상태일때만 State.Eat 재시도
    IEnumerator Retry()
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(5f);
            yield return new WaitUntil(() => animalControl.state == AnimalControl.State.Idle || animalControl.state == AnimalControl.State.Wander);
            // Idle, Wander 상태일때만 --> State.Eat 으로 체인지
            if (animalControl.state != AnimalControl.State.Eat)
                animalControl.ChangeState(AnimalControl.State.Eat);
        }
    }




    


}
