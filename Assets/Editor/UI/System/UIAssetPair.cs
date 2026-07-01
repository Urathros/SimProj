using UnityEngine;
using UnityEngine.UIElements;

public struct UIAssetPair
{
    public string Name { get; set; }
    public VisualTreeAsset Uxml { get; set; }
    public StyleSheet Uss { get; set; }

    public bool HasUxml => Uxml != null;
    public bool HasUss => Uss != null;

    public bool IsComplete => HasUxml && HasUss;

}
