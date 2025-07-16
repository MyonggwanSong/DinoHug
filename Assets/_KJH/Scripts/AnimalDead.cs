using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;
public class AnimalDead : AnimalAbility
{
    XROrigin xrOrigin;
    GameOverPanel gameOverPanel;
    protected override void Awake()
    {
        base.Awake();
        xrOrigin = FindAnyObjectByType<XROrigin>();
        gameOverPanel = FindAnyObjectByType<GameOverPanel>();
    }
    public override void Init()
    {
        StartCoroutine(nameof(Die));
    }
    public override void UnInit()
    {
        base.UnInit();
    }
    IEnumerator Die()
    {
        StartCoroutine(nameof(Look));
        animal.petStateController.UpdateIsInteraction(true);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        AudioManager.Instance.PlayEffect("DinoDie", transform.position, 0.6f);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);
        AudioManager.Instance.PlayEffect("GameOver", transform.position, 0.5f);
        anim.CrossFade("DieB", 0.2f);
        yield return YieldInstructionCache.WaitForSeconds(1.5f);
        Transform camTr = Camera.main.transform;
        Vector3 forward = camTr.forward;
        forward.y = 0f;
        forward.Normalize();
        gameOverPanel.transform.position = xrOrigin.transform.position + 1.9f * forward + 1.6f * Vector3.up;
        gameOverPanel.transform.forward = forward;
        gameOverPanel.Activate();
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
