using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class UICodeGenCache
{
    private const string UiGamePlayFilePath = "Assets/Resources/UI";
    private const string UiEditorFilePath = "Assets/Editor/UI";

    public List<VisualTreeAsset> EditorUxmlFiles { get; private set; } = null;
    public List<StyleSheet> EditorUssFiles { get; private set; } = null;
    public List<VisualTreeAsset> GamePlayUxmlFiles { get; private set; } = null;
    public List<StyleSheet> GamePlayUssFiles { get; private set; } = null;

    public string EditorFilePath => UiEditorFilePath;
    public string GamePlayFilePath => UiGamePlayFilePath;

    public UICodeGenCache()
    {
        if (EditorUxmlFiles == null) EditorUxmlFiles = new();
        if (EditorUssFiles == null) EditorUssFiles = new();

        if (GamePlayUxmlFiles == null) GamePlayUxmlFiles = new();
        if (GamePlayUssFiles == null) GamePlayUssFiles = new();

        Refresh();
    }

    private void LoadAssetsByExtension<T>(string extension, List<T> target, string filePath)
        where T : UnityEngine.Object
    {
        if (!AssetDatabase.IsValidFolder(filePath)) return;

        string[] guids = AssetDatabase.FindAssets("", new[] { filePath });
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

        EditorUxmlFiles.Clear();
        EditorUssFiles.Clear();

        GamePlayUxmlFiles.Clear();
        GamePlayUssFiles.Clear();

        LoadAssetsByExtension(".uxml", EditorUxmlFiles, UiEditorFilePath);
        LoadAssetsByExtension(".uss", EditorUssFiles, UiEditorFilePath);

        LoadAssetsByExtension(".uxml", GamePlayUxmlFiles, UiGamePlayFilePath);
        LoadAssetsByExtension(".uss", GamePlayUssFiles, UiGamePlayFilePath);

        Debug.Log($"UI Editor cache updated: {EditorUxmlFiles.Count} UXML, {EditorUssFiles.Count} USS");
        Debug.Log($"UI Gameplay cache updated: {GamePlayUxmlFiles.Count} UXML, {GamePlayUssFiles.Count} USS");
    }


}
