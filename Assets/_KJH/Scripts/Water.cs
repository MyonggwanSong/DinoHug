using UnityEngine;
public class Water : MonoBehaviour, IInteractable
{
    #region IInteractable Implement Setting
    public IInteractable.Type type => IInteractable.Type.Water;
    Transform IInteractable.root => transform.root;
    public bool _isPlace;
    bool IInteractable.isPlace => _isPlace;
    #endregion

    bool isGrab;
    void OnCollisionStay(Collision collision)
    {
        if (_isPlace) return;
        if (isGrab) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _isPlace = true;
        }
    }
    public void Grab()
    {
        isGrab = true;
        _isPlace = false;
    }
    public void UnGrab()
    {
        isGrab = false;
    }



}
