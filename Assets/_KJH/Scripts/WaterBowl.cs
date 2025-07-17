using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class WaterBowl : MonoBehaviour
{
    AnimalControl animalControl;
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
        animalControl = FindAnyObjectByType<AnimalControl>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        liquid = GetComponentInChildren<Liquid>();
        isPlaced = true;
    }
    float enableTime;
    void OnEnable()
    {
        enableTime = Time.time;
        coolTime = 0f;
    }
    public void OnGrabStart()
    {
        isGrabbed = true;
        isPlaced = false;
        StopCoroutine(nameof(Retry));
        AudioManager.Instance.PlayEffect("Grab", transform.position, 0.8f);
    }
    public void OnGrabEnd()
    {
        isGrabbed = false;
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if (!isGrabbed)
            {
                isPlaced = true;
            }
            if (isPlaced)
            {
                if (liquid.fillAmount >= Mathf.Lerp(fillRange.x, fillRange.y, 0.5f))
                {
                    // Idle, Wander 상태일때만 --> State.Drink 으로 체인지
                    if (animalControl.state == AnimalControl.State.Idle || animalControl.state == AnimalControl.State.Wander)
                        if (animalControl.state != AnimalControl.State.Drink)
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
            yield return YieldInstructionCache.WaitForSeconds(5.5f);
            yield return new WaitUntil(() => animalControl.state == AnimalControl.State.Idle || animalControl.state == AnimalControl.State.Wander);
            yield return new WaitUntil(() => !isRefuse);
            // Idle, Wander 상태일때만 --> State.Drink 으로 체인지
            if (animalControl.state == AnimalControl.State.Idle || animalControl.state == AnimalControl.State.Wander)
            {
                if (animalControl.state != AnimalControl.State.Drink)
                    animalControl.ChangeState(AnimalControl.State.Drink);
            }
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
        isPlaced = true;
        isGrabbed = false;
        StopCoroutine(nameof(Retry));
    }
    void OnCollisionEnter(Collision collision)
    {
        if (Time.time - enableTime < 2f) return;
        if (collision.gameObject.layer == 3)
        {
            if (coolTime > 0 && Time.time - coolTime < 3.5f) return;
            AudioManager.Instance.PlayEffect("Took", transform.position, 0.8f);
            ParticleManager.Instance.SpawnParticle(ParticleFlag.DustSmall, transform.position, Quaternion.identity, null);
            coolTime = Time.time;
        }
    }
    bool isRefuse;
    float coolTime;
    public void Refuse()
    {
        coolTime = Time.time;
        isRefuse = true;
        StartCoroutine(nameof(RefuseWait));
    }
    IEnumerator RefuseWait()
    {
        yield return YieldInstructionCache.WaitForSeconds(25f);
        isRefuse = false;
    }





}
