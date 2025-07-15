using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;
public class AnimalGameClear : AnimalAbility
{
    XROrigin xrOrigin;
    GameClearPanel gameClearPanel;
    protected override void Awake()
    {
        base.Awake();
        xrOrigin = FindAnyObjectByType<XROrigin>();
        gameClearPanel = FindAnyObjectByType<GameClearPanel>();
    }
    public override void Init()
    {
        StartCoroutine(nameof(Clear));
    }
    public override void UnInit()
    {
        base.UnInit();
    }
    IEnumerator Clear()
    {
        StartCoroutine(nameof(Look));
        animal.petStateController.currentState.bond = 100f;
        animal.petStateController.UpdateBond(animal.petStateController.currentState.bond);
        animal.petStateController.UpdateIsInteraction(true);
        AudioManager.Instance.PlayEffect("GameClear", transform.position, 0.5f);
        yield return YieldInstructionCache.WaitForSeconds(1f);
        anim.SetInteger("animation", 2);
        yield return YieldInstructionCache.WaitForSeconds(1.5f);
        Transform camTr = Camera.main.transform;
        Vector3 forward = camTr.forward;
        forward.y = 0f;
        forward.Normalize();
        gameClearPanel.transform.position = xrOrigin.transform.position + 1.9f * forward + 1.6f * Vector3.up;
        gameClearPanel.transform.forward = forward;
        gameClearPanel.Activate();
    }
    IEnumerator Look()
    {
        Transform camTr = Camera.main.transform;
        float startTime = Time.time;
        while (Time.time - startTime < 1.5f)
        {
            //camTr.rotation = Quaternion.Slerp(camTr.rotation, Quaternion.LookRotation(transform.position - camTr.position), Time.deltaTime);
            Vector3 vector = transform.position - camTr.position;
            vector.y = 0f;
            Vector3 forwardXZ = camTr.forward;
            forwardXZ.y = 0f;
            float angle = Quaternion.FromToRotation(forwardXZ, vector).eulerAngles.y;
            if (angle >= 5 && angle <= 180)
            {
                xrOrigin.RotateAroundCameraPosition(Vector3.up, 150f * Time.deltaTime);
            }
            else if (angle > 180 && angle < 355)
            {
                xrOrigin.RotateAroundCameraPosition(Vector3.up, -150f * Time.deltaTime);
            }
            yield return null;
        }
    }

    
    
}
