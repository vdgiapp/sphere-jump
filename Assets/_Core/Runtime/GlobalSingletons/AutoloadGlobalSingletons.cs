using UnityEngine;

public static class AutoloadGlobalSingletons
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitSingletons()
    {
        if (GameObject.Find("GlobalSingletons") != null) return;
        var prefab = Resources.Load<GameObject>("Prefabs/GlobalSingletons");
        if (prefab == null)
        {
            Debug.LogError("[AutoLoad] Cannot find GlobalSingletons prefab in Resources/Prefabs/");
            return;
        }
        var loader = Object.Instantiate(prefab);
        loader.name = "GlobalSingletons";
        Object.DontDestroyOnLoad(loader);
    }
}