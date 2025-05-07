using UnityEngine;

public class ScriptableObjectSingleton<T> : ScriptableObject where T: ScriptableObjectSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T[] assets = Resources.LoadAll<T>(""); // Load all assets type T
                if (assets == null || assets.Length <= 0)
                {
                    throw new System.Exception("Could not find any scriptable object singleton instances in the resources.");
                }
                else if (assets.Length > 1)
                {
                    Debug.LogWarning("Multiple instances of the scriptable object singleton found in the resources.");
                }
                _instance = assets[0];
            }
            return _instance;
        }
    }
}