using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T I
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();

                if (instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                }
            }

            return instance;
        }
    }
    protected abstract bool IsDontDestroy();

    protected virtual void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        if (IsDontDestroy())
            DontDestroyOnLoad(gameObject);
    }
}