using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class UICodeGenCache
{
    private const string UiFilePath = "Assets/UI";

    public List<VisualTreeAsset> UxmlFiles { get; private set; } = null;
    public List<StyleSheet> UssFiles { get; private set; } = null;

    public string FilePath => UiFilePath;

    public UICodeGenCache()
    {
        if (UxmlFiles == null) UxmlFiles = new();
        if (UssFiles == null) UssFiles = new();

        Refresh();
    }

    private void LoadAssetsByExtension<T>(string extension, List<T> target)
        where T : UnityEngine.Object
    {
        if (!AssetDatabase.IsValidFolder(UiFilePath)) return;

        string[] guids = AssetDatabase.FindAssets("", new[] { UiFilePath });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            if (!path.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) continue;

            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) target.Add(asset);
        }
    }

    public void Refresh()
    {

        UxmlFiles.Clear();
        UssFiles.Clear();

        LoadAssetsByExtension(".uxml", UxmlFiles);
        LoadAssetsByExtension(".uss", UssFiles);

        Debug.Log($"UI cache updated: {UxmlFiles.Count} UXML, {UssFiles.Count} USS");
    }


}
