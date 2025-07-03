using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public enum HandType
{
    Left,
    Right
}

public abstract class InteractionEventHandler : MonoBehaviour
{
    protected XRBaseInteractable interactable;

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

    public virtual void OnHoverEnter(HoverEnterEventArgs args) { }

    public virtual void OnHoverExit(HoverExitEventArgs args) { }
}