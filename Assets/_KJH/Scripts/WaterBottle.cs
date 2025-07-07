using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class WaterBottle : MonoBehaviour
{
    XRGrabInteractable xRGrab;
    bool isGrabbed;
    Vector3 startPosition;
    Quaternion startRotation;
    Rigidbody rigid;
    Transform grabbingXRController;
    int shakeCount = 0;
    void Awake()
    {
        TryGetComponent(out xRGrab);
        TryGetComponent(out rigid);
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    public void OnGrabStart()
    {
        // 아래 Transform을 xRGrab에서 어떻게 받아오는지.
        grabbingXRController = xRGrab.firstInteractorSelecting.transform;
        isGrabbed = true;
        StopCoroutine(nameof(Holding));
        StartCoroutine(nameof(Holding));
    }
    public void OnGrabEnd()
    {
        isGrabbed = false;
        StopCoroutine(nameof(Holding));
        grabbingXRController = null;
    }
    IEnumerator Holding()
    {
        yield return null;
        shakeCount = 0;
        bool isShakeUpStart = false;
        while (true)
        {
            float horizontal = Mathf.Abs(rigid.velocity.x) + Mathf.Abs(rigid.velocity.z);
            float angle = grabbingXRController.rotation.eulerAngles.z;
            // 컨트롤러를 기울이고 있을 경우 + 수평으로는 거의 안움직이고 있을경우
            if (angle >= 110 && angle <= 250 && horizontal < 2)
            {
                //Debug.Log($"angle : {grabbingXRController.rotation.eulerAngles.z}, vertical : {rigid.velocity.y}, horizontal : {horizontal}");
                //컨트롤러를 적당한 속도와 적당한 주기로 위아래(수직)으로 흔들경우
                if (rigid.velocity.y > 3.5f)
                {
                    if (shakeCount == 0)
                    {
                        isShakeUpStart = true;
                        shakeCount++;
                        if (co != null)
                        {
                            StopCoroutine(co);
                            co = null;
                        }
                        //Debug.Log(shakeCount);
                    }
                    else if (shakeCount > 0 && isShakeUpStart)
                    {
                        if (shakeCount % 2 == 0)
                        {
                            shakeCount++;
                            if (co != null)
                            {
                                StopCoroutine(co);
                                co = null;
                            }
                            //Debug.Log(shakeCount);
                        }
                    }
                    else if (shakeCount > 0 && !isShakeUpStart)
                    {
                        if (shakeCount % 2 == 1)
                        {
                            shakeCount++;
                            if (co != null)
                            {
                                StopCoroutine(co);
                                co = null;
                            }
                            //Debug.Log(shakeCount);
                        }
                    }
                }
                else if (rigid.velocity.y < -3.5f)
                {
                    if (shakeCount == 0)
                    {
                        isShakeUpStart = false;
                        shakeCount++;
                        if (co != null)
                        {
                            StopCoroutine(co);
                            co = null;
                        }
                        //Debug.Log(shakeCount);
                    }
                    else if (shakeCount > 0 && isShakeUpStart)
                    {
                        if (shakeCount % 2 == 1)
                        {
                            shakeCount++;
                            if (co != null)
                            {
                                StopCoroutine(co);
                                co = null;
                            }
                            //Debug.Log(shakeCount);
                        }
                    }
                    else if (shakeCount > 0 && !isShakeUpStart)
                    {
                        if (shakeCount % 2 == 0)
                        {
                            shakeCount++;
                            if (co != null)
                            {
                                StopCoroutine(co);
                                co = null;
                            }
                            //Debug.Log(shakeCount);
                        }
                    }
                }
                else
                {
                    if (co == null)
                    {
                        co = StartCoroutine(ResetShakeCount());
                    }
                }
            }
            else
            {
                if (co == null)
                {
                    co = StartCoroutine(ResetShakeCount());
                }
            }
            yield return null;
            if (shakeCount >= 4)
            {
                Debug.Log("물 따르기 시작");
                shakeCount = 0;
            }
        }
    }
    Coroutine co;
    IEnumerator ResetShakeCount()
    {
        yield return new WaitForSeconds(0.5f);
        shakeCount = 0;
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
        isGrabbed = false;
    }
    void FillOut()
    {
        

    }



}
