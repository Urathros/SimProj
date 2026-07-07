using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName ="Scriptables/UIAssetConfig")]
public class UIAssetConfig : ScriptableObject
{
    [field: SerializeField]
    public string Name { get; set; } = string.Empty;

    [field: SerializeField]
    public VisualTreeAsset Uxml { get; set; } = null;

    [field: SerializeField]
    public StyleSheet Uss { get; set; } = null;
    
    [field: SerializeField]
    public PanelSettings PanelSettings { get; set; } = null;

    public bool HasUxml => Uxml != null;
    public bool HasUss => Uss != null;

    public bool HasPanelSettings => PanelSettings != null;

    public bool IsComplete => HasUxml && HasUss && HasPanelSettings;

}
