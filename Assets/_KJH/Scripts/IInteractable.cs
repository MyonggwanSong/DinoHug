using UnityEngine;
public interface IInteractable
{
    public Type type { get; }
    public Transform root{ get; }
    public bool isPlace { get; }
    public enum Type
    {
        Food,
        Water,
        Toy,
    }

}
