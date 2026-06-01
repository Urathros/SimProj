using UnityEngine;

public static class SingletonDiagnostics
{
    public static void DuplicateDestroyed<T>(Object context)
    {
        Debug.LogWarning(
            $"Duplicate singleton of type {typeof(T).Name} destroyed.",
            context);
    }

    public static void MissingScriptableAsset<T>(string path)
    {
        Debug.LogError(
            $"Missing ScriptableSingleton asset: {typeof(T).Name}. Expected at Resources/{path}.asset");
    }

    public static void RequestedWhileQuitting<T>()
    {
        Debug.LogWarning(
            $"[{typeof(T).Name}] Instance requested while application is quitting.");
    }
}
