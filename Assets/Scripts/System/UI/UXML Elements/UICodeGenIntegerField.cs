using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("IntegerField", libraryPath = "Codegen")]
public partial class UICodeGenIntegerField : IntegerField, IUICodeGenBindable
{
    [UxmlAttribute("codegen-bind-name")]
    public string BindName { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-mode")]
    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;

    public string BindingTargetProperty => nameof(value);
}
