using UnityEngine;
using UnityEngine.EventSystems;

public class HandleXRInteract : MonoBehaviour, IEndDragHandler
{
    public void OnEndDrag(PointerEventData eventData)
    {
        SFX effectSfx = AudioManager.Instance.PlayEffect("UIClick1", transform.position, 0f);
    }
}
