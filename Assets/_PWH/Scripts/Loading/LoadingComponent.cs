using UnityEngine;

public class LoadingComponent : MonoBehaviour
{
    [SerializeField] string offsetName;
    [SerializeField, ReadOnlyInspector] Transform offset;

    void Start()
    {
        SetOffset();
    }

    void OnEnable()
    {
        SetOffset();
    }

    void SetOffset()
    {
        offset = GameObject.Find(offsetName).transform;

        if (offset == null) return;

        this.gameObject.transform.SetParent(offset);
        this.gameObject.transform.localPosition = Vector3.zero;
    }
}
