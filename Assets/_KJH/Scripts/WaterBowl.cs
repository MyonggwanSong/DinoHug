using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class WaterBowl : MonoBehaviour
{
    public AnimalControl animalControl;
    public bool isPlaced;
    public Vector2 fillRange = new Vector2(0.4f, 0.8f);
    XRGrabInteractable xRGrab;
    bool isGrabbed;
    Vector3 startPosition;
    Quaternion startRotation;
    [HideInInspector] public Liquid liquid;
    void Awake()
    {
        TryGetComponent(out xRGrab);
        startPosition = transform.position;
        startRotation = transform.rotation;
        liquid = GetComponentInChildren<Liquid>();
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
                if (liquid.fillAmount >= Mathf.Lerp(fillRange.x, fillRange.y, 0.5f))
                {
                    animalControl.ChangeState(AnimalControl.State.Drink);
                    StartCoroutine(nameof(Retry));
                }
            }
        }
    }
    IEnumerator Retry()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            yield return new WaitUntil(() => animalControl.state == AnimalControl.State.Idle || animalControl.state == AnimalControl.State.Wander);
            animalControl.ChangeState(AnimalControl.State.Drink);
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
