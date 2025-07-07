using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;

public class ObjectOutliner : MonoBehaviour
{
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private InteractionEventHandler eventHandler;

    [Header("Outline Material")]
    [SerializeField] Material outlineFill;
    [SerializeField] Material outlineMask;

    void Awake()
    {
        TryGetComponent(out renderer);
        TryGetComponent(out eventHandler);

        InitRenderer();
    }

    void OnEnable()
    {
        eventHandler.OnHoverCheck += SetActiveOutline;
    }

    void OnDisable()
    {
        eventHandler.OnHoverCheck -= SetActiveOutline;  
    }

    void InitRenderer()
    {
        if (renderer == null) return;

        Debug.Log("Init Renderer");
        
        outlineFill.SetInt("_OutlineWidth", 0);
        renderer.AddMaterial(outlineFill);
        renderer.AddMaterial(outlineMask);
    }

    public void SetActiveOutline(bool on)
    {
        if (on)
        {
            outlineFill.SetInt("_OutlineWidth", 10);
        }
        else
        {
            outlineFill.SetInt("_OutlineWidth", 0);
        }
    }
}