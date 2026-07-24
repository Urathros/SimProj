using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("RadioButtonGroup", libraryPath = "Codegen")]
public partial class UICodeGenRadioButtonGroup : RadioButtonGroup, IUICodeGenBindable, IUICodeGenCommandSource
{
    [UxmlAttribute("codegen-bind-name")]
    public string BindName { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-mode")]
    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;

    [UxmlAttribute("codegen-command")]
    public string CommandName { get; set; } = string.Empty;

    public string BindingTargetProperty => nameof(value);
}