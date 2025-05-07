using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    public static T Instance { get; private set; }
    private static bool _quitting = false;

    protected virtual void Awake()
    {
        if (_quitting) return;
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
            OnSingletonAwake();
        }
        else if (Instance != this)
        {
            Debug.LogWarning($"[GSingleton] Duplicate {typeof(T).Name} found. Destroying this one.");
            Destroy(gameObject);
        }
    }

    protected virtual void OnSingletonAwake() { }

    protected virtual void OnApplicationQuit()
    {
        _quitting = true;
    }
}