using UnityEngine;
using Unity.Netcode;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance;
    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}

public class NetSingleton<T> : NetworkBehaviour where T : Component
{
    public static T Instance;
    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}