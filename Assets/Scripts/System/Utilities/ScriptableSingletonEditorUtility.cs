#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

internal static class ScriptableSingletonEditorUtility
{
    public static T CreateAsset<T>(string assetPath)
        where T : ScriptableObject
    {
        EnsureDirectory(Path.GetDirectoryName(assetPath));

        var asset = ScriptableObject.CreateInstance<T>();

        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorGUIUtility.PingObject(asset);

        return asset;
    }

    public static void EnsureSingleAsset<T>()
        where T : ScriptableObject
    {
        var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        if (guids.Length <= 1) return;

        Debug.LogWarning($@"Multiple assets of type {typeof(T).Name} found.
Keep only one ScriptableSingleton asset.");
    }

    private static void EnsureDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        if (AssetDatabase.IsValidFolder(path)) return;

        var folders = path.Split('/');
        var current = folders[0];

        for (int i = 1; i < folders.Length; i++)
        {
            var next = $"{current}/{folders[i]}";

            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, folders[i]);

            current = next;
        }
    }
}

#endif