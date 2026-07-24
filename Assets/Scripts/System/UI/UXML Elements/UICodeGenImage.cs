using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

[UxmlElement("Image", libraryPath = "Codegen")]

public partial class UICodeGenImage : Image, IUICodeGenBindable, IUICodeGenClickable
{

    [UxmlAttribute("codegen-bind-name")]
    public string BindName { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-mode")]
    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;

    public string BindingTargetProperty => nameof(image);

    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;
}
