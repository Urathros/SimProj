using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

[UxmlElement("Label", libraryPath = "Codegen")]
public partial class UICodeGenLabel : Label, IUICodeGenBindable
{
    [UxmlAttribute("codegen-bind-name")]
    public string BindName { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-mode")]
    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;

    public string BindingTargetProperty => nameof(text);
}
