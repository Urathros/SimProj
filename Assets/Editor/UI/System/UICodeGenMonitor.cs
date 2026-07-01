using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public sealed class UICodeGenMonitor : AssetPostprocessor
{
    private static UICodeGenCache _cache = null;
    private static UICodeGenService _service = null;

    public static bool IsGenerating { get; set; } = false;
    static UICodeGenMonitor()
    {
        Debug.Log("Initialized");
        _cache = new();
        _service = new();
    }

    private static bool AnyUIAssetChanged(string[] paths)
    {
        foreach (var path in paths)
        {
            if (!path.StartsWith( $"{_cache.EditorFilePath}/", StringComparison.OrdinalIgnoreCase)) continue;

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
            try
            {
                IsGenerating = true;

                _cache.Refresh();
                foreach (var editorUxml in _cache.EditorUxmlFiles)
                    _service.GenerateEditorFile(editorUxml);
                AssetDatabase.Refresh();
            }
            catch (Exception ex) { Debug.LogError(ex.Message); }
            finally { IsGenerating = false; }
        }
    }
}
