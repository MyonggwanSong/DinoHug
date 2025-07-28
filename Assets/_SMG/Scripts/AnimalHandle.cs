using System.Collections;
using UnityEngine;
public class AnimalHandle : AnimalAbility
{
    [HideInInspector] public bool isPlaying = false;
    [ReadOnlyInspector] public Transform controllerTr;
    [SerializeField] float coolTime = 0.2f; // 쓰다듬기 성공시.. 다시 성공하려면 쿨타임을 기다려야함
    Transform camearaTr;

    // 컨트롤러와 플레이어의 속도 계산을 위한 멤버변수
    Vector3 prevPosition;
    Vector3 currPosition;
    Vector3 prevPlayerPosition;
    Vector3 currPlayerPosition;
    Vector3 velocity;
    Vector3 velocityPlayer;

    public override void Init()
    {
        camearaTr = Camera.main.transform;
        StartCoroutine(nameof(StartLoop));
        animal.HeadIK_OFF();
        coCoolTime = null;
        coSuccess = null;
        animal.petStateController.UpdateIsInteraction(false);
    }
    public override void UnInit()
    {
        base.UnInit();
        StopCoroutine(nameof(StartLoop));
        anim.SetInteger("animation", 1); // Idle 모션
        animal.ChangeFace(AnimalControl.Face.Default); // 원래 표정으로
        animal.HeadIK_ON();
    }
    IEnumerator StartLoop()
    {
        prevPosition = controllerTr.position;
        currPosition = controllerTr.position;
        prevPlayerPosition = camearaTr.position;
        currPlayerPosition = camearaTr.position;
        while (true)
        {
            yield return null;
            UpdatePosition();
            if (velocityPlayer.sqrMagnitude < 0.1f * 0.1f
            && velocity.sqrMagnitude > 0.01f * 0.01f
            && Mathf.Abs(velocity.y) < 0.5f)
            {
                if (!isCoolTime)
                {
                    Debug.Log($"쓰다 듬기 성공");
                    Vector3 particlePos = controllerTr.position;
                    animal.petStateController.Petting(); // 유대감 증가
                    PoolableParticle pp = ParticleManager.Instance.SpawnParticle(ParticleFlag.Petting, particlePos, Quaternion.identity, null);
                    pp.transform.localScale = Random.Range(0.4f,0.8f) * Vector3.one;
                    pp.transform.localRotation = Random.rotation;
                    if (coSuccess == null)
                    {
                        coSuccess = StartCoroutine(SuccessAnimation());
                    }
                }
                // 쓰다듬기 성공시.. 다시 성공하려면 0.2초 쿨타임 기다려야함
                if (coCoolTime == null)
                {
                    coCoolTime = StartCoroutine(CoolTime());
                }
            }
        }
    }
    bool isCoolTime;
    Coroutine coCoolTime;
    IEnumerator CoolTime()
    {
        isCoolTime = true;
        yield return new WaitForSeconds(coolTime);
        isCoolTime = false;
        coCoolTime = null;
    }
    Coroutine coSuccess;
    IEnumerator SuccessAnimation()
    {
        anim.SetInteger("animation", 2); // 행복한 모션
        animal.ChangeFaceTemporal(AnimalControl.Face.Joyful, 1f); // 표정 변화
        AudioManager.Instance.PlayEffect("Happy", transform.position + Vector3.up * 1.2f, 1f); // SFX
        Vector3 _particleOsset = transform.position + new Vector3(0f, 1.2f, 0f);
        ParticleManager.Instance.SpawnParticle(ParticleFlag.Twinkle, _particleOsset, Quaternion.identity, null); // particle
        AudioManager.Instance.PlayEffect("Petting", transform.position + Vector3.up * 1.2f, 1f); // SFX
        yield return new WaitForSeconds(2f);
        anim.SetInteger("animation", 1); // Idle 모션
        animal.ChangeFace(AnimalControl.Face.Default); // 원래 표정으로
        yield return new WaitForSeconds(0.2f);
        coSuccess = null;
    }
    void UpdatePosition()
    {
        prevPosition = currPosition;
        currPosition = controllerTr.position;
        velocity = currPosition - prevPosition;
        prevPlayerPosition = currPlayerPosition;
        currPlayerPosition = camearaTr.position;
        velocityPlayer = currPlayerPosition - prevPlayerPosition;
    }
    

}
