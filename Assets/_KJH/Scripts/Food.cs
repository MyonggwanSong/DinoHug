using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Food : MonoBehaviour
{
    public AnimalControl animalControl;
    XRGrabInteractable xRGrab;
    public bool isPlaced;
    bool isGrabbed;
    Vector3 startPosition;
    Quaternion startRotation;
    void Awake()
    {
        TryGetComponent(out xRGrab);
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
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if (isPlaced) return;
            if (!isGrabbed)
            {
                isPlaced = true;
                animalControl.ChangeState(AnimalControl.State.Eat);
                StartCoroutine(nameof(Retry));
            }
        }
    }
    IEnumerator Retry()
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(2f);
            yield return new WaitUntil(() => animalControl.state == AnimalControl.State.Idle || animalControl.state == AnimalControl.State.Wander);
            animalControl.ChangeState(AnimalControl.State.Eat);
        }
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

    


}
