using Unity.XR.CoreUtils;
using UnityEngine;

public class ObjectOutlineControl : MonoBehaviour
{
    [SerializeField] private InteractionEventHandler eventHandler;
    [SerializeField, ReadOnlyInspector] private Outline outline;

    void Awake()
    {
        if (TryGetComponent(out outline))
        {
            outline.enabled = false;
        }
        TryGetComponent(out eventHandler);
    }

    void OnEnable()
    {
        eventHandler.OnHoverCheck += SetActiveOutline;
    }

    void OnDisable()
    {
        eventHandler.OnHoverCheck -= SetActiveOutline;
    }

    public void SetActiveOutline(bool on)
    {
        if (on)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        } 
    }
}