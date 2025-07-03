using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RayInteractionEvent : InteractionEventHandler
{
    XRRayInteractor rayInteractor = null;
 
    public override void OnHoverEnter(HoverEnterEventArgs args)
    {
        base.OnHoverEnter(args);

        rayInteractor = args.interactorObject as XRRayInteractor;

        if (rayInteractor == null) return;

        if (rayInteractor.TryGetHitInfo(out Vector3 hitPoint, out Vector3 hitNormal, out int _, out bool _))
        {
            OnHoverCheck?.Invoke(true);
        }
    }

    public override void OnHoverExit(HoverExitEventArgs args)
    {
        base.OnHoverExit(args);

        rayInteractor = null;
        OnHoverCheck?.Invoke(false);
    }

    void Update()
    {
        if (rayInteractor == null) return;

        if (rayInteractor.TryGetHitInfo(out Vector3 hitPoint, out Vector3 hitNormal, out int _, out bool _))
        {
            OnHoverPointUpdate?.Invoke(hitPoint);
        }
    }
}