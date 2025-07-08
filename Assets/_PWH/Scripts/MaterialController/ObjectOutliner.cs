using Unity.XR.CoreUtils;
using UnityEngine;

public class ObjectOutliner : MonoBehaviour
{
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private InteractionEventHandler eventHandler;

    [Header("Outline Material Prefab")]
    [SerializeField] Material outlineFill_Prefab;
    [SerializeField] Material outlineMask_Prefab;

    private Material outlineFill;

    void Awake()
    {
        if (!TryGetComponent(out renderer))
        {
            renderer = GetComponentInChildren<MeshRenderer>();
        }
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

        outlineFill = Instantiate(outlineFill_Prefab);

        if (outlineFill == null) return;
        
        renderer.AddMaterial(outlineFill);
        renderer.AddMaterial(outlineMask_Prefab);
        outlineFill.SetInt("_OutlineWidth", 0);
    }

    public void SetActiveOutline(bool on)
    {
        if (on)
        {
            //현재의 fill에서 가져오기
            outlineFill.SetInt("_OutlineWidth", 10);
        }
        else
        {
            outlineFill.SetInt("_OutlineWidth", 0);
        }
    }
}