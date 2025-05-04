using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _instanceLock = new();
    private static bool _quitting = false;

    public static T Instance
    {
        get
        {
            if (_quitting) return null;
            lock (_instanceLock)
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;
                GameObject gameObj = new(typeof(T).Name);
                _instance = gameObj.AddComponent<T>();
                DontDestroyOnLoad(gameObj);
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            OnSingletonAwake();
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[MonoBehaviourSingleton] Instance of {typeof(T).Name} already exists. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnSingletonAwake() { }

    protected virtual void OnApplicationQuit()
    {
        _quitting = true;
    }
}