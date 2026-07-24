using UnityEngine;
using UnityEngine.UIElements;


[UxmlElement("FloatField", libraryPath = "Codegen")]
public partial class UICodeGenFloatField : FloatField, IUICodeGenBindable
{
    [UxmlAttribute("codegen-bind-name")]
    public string BindName { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-mode")]
    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;

    public string BindingTargetProperty => nameof(value);
}
