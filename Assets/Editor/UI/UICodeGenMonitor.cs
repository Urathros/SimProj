using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public sealed class UICodeGenMonitor : AssetPostprocessor
{
    private static UICodeGenCache _cache = null;

    static UICodeGenMonitor()
    {
        Debug.Log("Initialized");
        _cache = new();
    }

    private static bool AnyUIAssetChanged(string[] paths)
    {
        foreach (var path in paths)
        {
            if (!path.StartsWith( $"{_cache.FilePath}/", StringComparison.OrdinalIgnoreCase)) continue;

            var ext = Path.GetExtension(path).ToLowerInvariant();

            if (ext == ".uxml" || ext == ".uss") return true;
        }

        return false;
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (AnyUIAssetChanged(importedAssets) ||
            AnyUIAssetChanged(deletedAssets) ||
            AnyUIAssetChanged(movedAssets) ||
            AnyUIAssetChanged(movedFromAssetPaths))
        {
            _cache.Refresh();
        }
    }
}
