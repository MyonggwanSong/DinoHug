using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class InteractionEventHandler : MonoBehaviour
{
    protected int handCount = 0;
    protected XRBaseInteractable interactable;
    public XRBaseInteractor interactor;

    public UnityAction<bool> OnHoverCheck;
    public UnityAction<Vector3> OnHoverPointUpdate;

    void Awake()
    {
        interactable = GetComponentInChildren<XRBaseInteractable>();
    }

    void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
    }

    void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
    }

    protected virtual void OnHoverEnter(HoverEnterEventArgs args)
    {
        handCount++;
        this.interactor = args.interactorObject as XRBaseInteractor;
    }

    protected virtual void OnHoverExit(HoverExitEventArgs args)
    {
        handCount--;
        interactable = null;
    }
}