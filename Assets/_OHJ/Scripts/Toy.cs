using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Toy : MonoBehaviour
{
    public AnimalControl animalControl;
    XRGrabInteractable xRGrab;
    public bool isGrab;
    public bool isThrow;
    public bool isTrackable;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider coll;
    [Header("공 던지는 힘,각도")]
    public float force = 5f;
    public float angle = 30f;
    private void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out coll);
        TryGetComponent(out xRGrab);
    }
    public void GrabStart()
    {
        AudioManager.Instance.PlayEffect("Grab", transform.position, 0.8f);
        isGrab = true;

        // 상태 우선순위 처리
        if (animalControl.state == AnimalControl.State.Play)
        {
            Debug.Log("이미 Play 상태인데 그랩해서 Play상태 재시도");
        }
        if (animalControl.state == AnimalControl.State.Handle) return;
        if (animalControl.state == AnimalControl.State.Eat) return;
        if (animalControl.state == AnimalControl.State.Drink) return;
        animalControl.ChangeState(AnimalControl.State.Play);
    }
    public void GrabEnd()
    {
        isGrab = false;
        isThrow = true;
        Throwarc();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            AudioManager.Instance.PlayEffect("DollSound", transform.position, 1f);
            ParticleManager.Instance.SpawnParticle(ParticleFlag.Dust, transform.position, Quaternion.identity, null);
        }
    }

    public void Throwarc()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) { return; }
        Vector3 forward = transform.forward.normalized;
        float rad = angle * Mathf.Deg2Rad;
        float horzForce = Mathf.Cos(rad) * force;   //수평
        float vertForce = Mathf.Sin(rad) * force;   //수직
        Vector3 init_vel = horzForce * forward + Vector3.up * vertForce;
        rb.AddForce(init_vel, ForceMode.VelocityChange);
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
    }






















    //public AnimalControl animal;
    //public XRGrabInteractable xRGrab;
    //public Throw t;

    //public bool isPlaced;
    //public bool isGrabbed;
    //public bool isThrow;

    //public void GrabStart()
    //{
    //    isGrabbed = true;
    //    isPlaced = false;
    //    isThrow = false;
    //    animal.ChangeState(AnimalControl.State.Play);
    //    StopCoroutine(nameof(Retry));
    //    StartCoroutine(nameof(Retry));
    //}
    //public void GrabEnd()
    //{
    //    isGrabbed = false;
    //}

    //private IEnumerator Retry()
    //{
    //    while(true)
    //    {
    //        yield return YieldInstructionCache.WaitForSeconds(0.5f);
    //        yield return new WaitUntil(() => animal.state == AnimalControl.State.Idle || animal.state == AnimalControl.State.Wander);
    //        animal.ChangeState(AnimalControl.State.Play);
    //    }
    //}

    //public void OnDisableGrab()
    //{
    //    xRGrab.enabled = false;
    //}



    //private Rigidbody rb;
    //private Vector3 init_vel;
    //Toy toy;

    //private void Awake()
    //{
    //    TryGetComponent(out toy);
    //}

    //private void Update()
    //{
    //    if (transform.position.y < 0)
    //    {
    //        Vector3 pos = transform.position;
    //        pos.y = 0;
    //        transform.position = pos;
    //    }
    //}



}
