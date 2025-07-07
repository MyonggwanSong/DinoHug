using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class CustomDirectInterator : XRDirectInteractor
{
    void Start()
    {
        
    }
    protected override void OnHoverEntered(XRBaseInteractable interactable)
    {
        base.OnHoverEntered(interactable);
    }
    protected override void OnHoverExited(XRBaseInteractable interactable)
    {
        base.OnHoverExited(interactable);
    }
}
