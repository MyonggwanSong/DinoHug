using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RayInteractionEvent : InteractionEventHandler
{
    XRRayInteractor rayInteractor = null;

    private float elapsed = 0f;
    [SerializeField] float spawnTime = 0.02f;

    void Update()
    {
        if (rayInteractor == null) return;

        //0.5초 마다 파티클 오브젝트 생성하기
        if (rayInteractor.TryGetHitInfo(out Vector3 hitPoint, out Vector3 hitNormal, out int _, out bool _))
        {
            elapsed += Time.deltaTime;
            if (elapsed >= spawnTime)
            {
                OnHoverPointUpdate?.Invoke(hitPoint);
                elapsed = 0;
            }
        }
    }
     
    protected override void OnHoverEnter(HoverEnterEventArgs args)
    {
        base.OnHoverEnter(args);

        rayInteractor = args.interactorObject as XRRayInteractor;

        if (rayInteractor == null) return;

        if (rayInteractor.TryGetHitInfo(out Vector3 hitPoint, out Vector3 hitNormal, out int _, out bool _))
        {
            OnHoverCheck?.Invoke(true);
        }
    }

    protected override void OnHoverExit(HoverExitEventArgs args)
    {
        base.OnHoverExit(args);

        rayInteractor = null;
        OnHoverCheck?.Invoke(false);
        elapsed = 0f;
    }
}