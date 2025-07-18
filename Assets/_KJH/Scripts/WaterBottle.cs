using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class WaterBottle : MonoBehaviour
{
    AnimalControl animalControl;
    XRGrabInteractable xRGrab;
    bool isGrabbed;
    Vector3 startPosition;
    Quaternion startRotation;
    Rigidbody rigid;
    Transform grabbingXRController;
    int shakeCount = 0;
    GameObject particleObj;
    ParticleSystem particle;
    Transform childParticle;
    bool isChangeState = false;
    void Awake()
    {
        TryGetComponent(out xRGrab);
        TryGetComponent(out rigid);
        animalControl = FindAnyObjectByType<AnimalControl>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        particleObj = transform.Find("Bottle").GetChild(0).gameObject;
        particleObj.TryGetComponent(out particle);
        childParticle = particleObj.transform.GetChild(0);
    }
    public void OnGrabStart()
    {
        // 아래 Transform을 xRGrab에서 어떻게 받아오는지.
        grabbingXRController = xRGrab.firstInteractorSelecting.transform;
        isGrabbed = true;
        StopCoroutine(nameof(Holding));
        StartCoroutine(nameof(Holding));
        isChangeState = false;
        AudioManager.Instance.PlayEffect("Grab", transform.position);
    }
    void OnEnable()
    {
        coolTime = 0f;
    }
    public void OnGrabEnd()
    {
        isGrabbed = false;
        StopCoroutine(nameof(Holding));
        grabbingXRController = null;
        if (coResetShake != null)
        {
            StopCoroutine(coResetShake);
            coResetShake = null;
        }
        StopWaterFillOut();
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
            if (angle >= 90 && angle <= 270 && horizontal < 6)
            {
                //Debug.Log($"angle : {grabbingXRController.rotation.eulerAngles.z}, vertical : {rigid.velocity.y}, horizontal : {horizontal}");
                //컨트롤러를 적당한 속도와 적당한 주기로 위아래(수직)으로 흔들경우
                if (rigid.velocity.y > 3f)
                {
                    if (shakeCount == 0)
                    {
                        isShakeUpStart = true;
                        shakeCount++;
                        if (coResetShake != null)
                        {
                            StopCoroutine(coResetShake);
                            coResetShake = null;
                        }
                        //Debug.Log(shakeCount);
                    }
                    else if (shakeCount > 0 && isShakeUpStart)
                    {
                        if (shakeCount % 2 == 0)
                        {
                            shakeCount++;
                            if (coResetShake != null)
                            {
                                StopCoroutine(coResetShake);
                                coResetShake = null;
                            }
                            //Debug.Log(shakeCount);
                        }
                    }
                    else if (shakeCount > 0 && !isShakeUpStart)
                    {
                        if (shakeCount % 2 == 1)
                        {
                            shakeCount++;
                            if (coResetShake != null)
                            {
                                StopCoroutine(coResetShake);
                                coResetShake = null;
                            }
                            //Debug.Log(shakeCount);
                        }
                    }
                }
                else if (rigid.velocity.y < -3f)
                {
                    if (shakeCount == 0)
                    {
                        isShakeUpStart = false;
                        shakeCount++;
                        if (coResetShake != null)
                        {
                            StopCoroutine(coResetShake);
                            coResetShake = null;
                        }
                        //Debug.Log(shakeCount);
                    }
                    else if (shakeCount > 0 && isShakeUpStart)
                    {
                        if (shakeCount % 2 == 1)
                        {
                            shakeCount++;
                            if (coResetShake != null)
                            {
                                StopCoroutine(coResetShake);
                                coResetShake = null;
                            }
                            //Debug.Log(shakeCount);
                        }
                    }
                    else if (shakeCount > 0 && !isShakeUpStart)
                    {
                        if (shakeCount % 2 == 0)
                        {
                            shakeCount++;
                            if (coResetShake != null)
                            {
                                StopCoroutine(coResetShake);
                                coResetShake = null;
                            }
                            //Debug.Log(shakeCount);
                        }
                    }
                }
                else
                {
                    if (coResetShake == null)
                    {
                        coResetShake = StartCoroutine(ResetShakeCount());
                    }
                }
            }
            else
            {
                if (coResetShake == null)
                {
                    coResetShake = StartCoroutine(ResetShakeCount());
                }
                if (!(angle >= 90 && angle <= 270))
                    StopWaterFillOut();
            }
            yield return null;
            if (shakeCount >= 3)
            {
                if (coWaterFill == null)
                {
                    //Debug.Log("물 따르기 시작");
                    coWaterFill = StartCoroutine(WaterFillOut());
                }
                shakeCount = 0;
            }
        }
    }
    Coroutine coResetShake;
    IEnumerator ResetShakeCount()
    {
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
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
    Coroutine coWaterFill;
    SFX sfx;
    IEnumerator WaterFillOut()
    {
        sfx = null;
        yield return YieldInstructionCache.WaitForSeconds(0.7f);
        sfx = AudioManager.Instance.PlayEffect("WaterFill", transform.position);
        particleObj.SetActive(true);
        particle.Stop(true);
        particle.Play();
        Ray ray = new Ray();
        RaycastHit hit;
        Collider[] colliders = new Collider[10];
        ray.direction = Vector3.down;
        WaterBowl bowl = null;
        isChangeState = false;
        while (true)
        {
            ray.origin = particleObj.transform.position;
            bowl = null;
            if (Physics.Raycast(ray, out hit, 100f, 1 << 3, QueryTriggerInteraction.Ignore))
            {
                childParticle.position = hit.point + 0.1f * Vector3.up;
                childParticle.up = hit.normal;
                int count = Physics.OverlapSphereNonAlloc(hit.point, 0.2f, colliders);
                for (int i = 0; i < count; i++)
                {
                    if (colliders[i].TryGetComponent(out bowl))
                    {
                        break;
                    }
                }
            }
            if (bowl == null)
            {
                //Debug.Log("아래에 Bowl이 없습니다.");
                //StopWaterFillOut();
            }
            else
            {
                float range = bowl.fillRange.y - bowl.fillRange.x;
                bowl.liquid.fillAmount += range * 0.01f;
                bowl.liquid.fillAmount = Mathf.Clamp(bowl.liquid.fillAmount, bowl.fillRange.x, bowl.fillRange.y);
                if (bowl.liquid.fillAmount >= 0.3f * bowl.fillRange.x + 0.7f * bowl.fillRange.y && !isChangeState)
                {
                    // Idle, Wander 상태일때만 --> State.Drink로 체인지
                    if (animalControl.state == AnimalControl.State.Idle || animalControl.state == AnimalControl.State.Wander)
                    {
                        if (animalControl.state != AnimalControl.State.Drink)
                        {
                            isChangeState = true;
                            animalControl.ChangeState(AnimalControl.State.Drink);
                        }
                    }
                }
                else if (bowl.liquid.fillAmount >= bowl.fillRange.y)
                {
                    StopWaterFillOut();
                }
            }
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }
    }
    public void StopWaterFillOut()
    {
        if (coWaterFill != null)
        {
            sfx?.Stop();
            //Debug.Log("물 따르기 끝");
            particle.Stop(true);
            particleObj.SetActive(false);
            StopCoroutine(coWaterFill);
            coWaterFill = null;
        }
    }
    float coolTime = 0f;
    void OnCollisionEnter(Collision collision)
    {
        if (isGrabbed) return;
        if (collision.gameObject.layer == 3)
        {
            if (coolTime > 0 && Time.time - coolTime < 4.5f) return;
            AudioManager.Instance.PlayEffect("Took", transform.position);
            ParticleManager.Instance.SpawnParticle(ParticleFlag.DustSmall, transform.position, Quaternion.identity, null);
            coolTime = Time.time;
        }
    }



}
