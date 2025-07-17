using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Food : MonoBehaviour
{
    AnimalControl animalControl;
    XRGrabInteractable xRGrab;
    public bool isPlaced;
    [HideInInspector] public bool isRefuse;
    bool isGrabbed;
    Vector3 startPosition;
    Quaternion startRotation;
    Collider collider;
    Rigidbody rigidbody;
    void Awake()
    {
        TryGetComponent(out xRGrab);
        TryGetComponent(out collider);
        TryGetComponent(out rigidbody);
        animalControl = FindAnyObjectByType<AnimalControl>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    public void OnGrabStart()
    {
        AudioManager.Instance.PlayEffect("Grab", transform.position);
        isGrabbed = true;
        isPlaced = false;
        StopCoroutine(nameof(Retry));
        StopCoroutine(nameof(RefuseWait));
        isRefuse = false;
    }
    public void OnGrabEnd()
    {
        isGrabbed = false;
    }
    public void DisableGrab()
    {
        xRGrab.enabled = false;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        //collider.enabled = false;
    }
    public void EnableGrab()
    {
        xRGrab.enabled = true;
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
        //collider.enabled = true;
    }
    public void Reset()
    {
        EnableGrab();
        transform.position = startPosition;
        transform.rotation = startRotation;
        isPlaced = false;
        isGrabbed = false;
        StopCoroutine(nameof(Retry));
        StopCoroutine(nameof(RefuseWait));
        StartCoroutine(Reset_co());
    }
    IEnumerator Reset_co()
    {
        xRGrab.enabled = false;
        for (int i = 0; i < 10; i++)
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
            transform.position = startPosition;
            transform.rotation = startRotation;
        }
        xRGrab.enabled = true;
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
                    if (animalControl.state != AnimalControl.State.Eat)
                        animalControl.ChangeState(AnimalControl.State.Eat);
                }
                StartCoroutine(nameof(Retry));
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if (coolTime > 0 && Time.time - coolTime < 3.5f) return;
            AudioManager.Instance.PlayEffect("Took", transform.position);
            ParticleManager.Instance.SpawnParticle(ParticleFlag.Dust, transform.position, Quaternion.identity, null);
            coolTime = Time.time;
        }
    }
    // 푸드가 바닥에 떨어져있고 + Idle, Wander 상태일때만 State.Eat 재시도
    IEnumerator Retry()
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(5.5f);
            yield return new WaitUntil(() => animalControl.state == AnimalControl.State.Idle || animalControl.state == AnimalControl.State.Wander);
            yield return new WaitUntil(() => !isRefuse);
            // Idle, Wander 상태일때만 --> State.Eat 으로 체인지
            if (animalControl.state == AnimalControl.State.Idle || animalControl.state == AnimalControl.State.Wander)
            {
                if (animalControl.state != AnimalControl.State.Eat)
                    animalControl.ChangeState(AnimalControl.State.Eat);
            }
        }
    }
    public void Refuse()
    {
        coolTime = Time.time;
        isRefuse = true;
        StartCoroutine(nameof(RefuseWait));
    }
    IEnumerator RefuseWait()
    {
        yield return YieldInstructionCache.WaitForSeconds(30f);
        isRefuse = false;
    }
    float coolTime = 0f;
    void OnEnable()
    {
        coolTime = 0f;
    }




    


}
