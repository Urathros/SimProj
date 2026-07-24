using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("Foldout", libraryPath = "Codegen")]
public partial class UICodeGenFoldout : Foldout, IUICodeGenBindable, IUICodeGenCommandSource
{
    [UxmlAttribute("codegen-bind-name")]
    public string BindName { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-mode")]
    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;

    public string BindingTargetProperty => nameof(value);


    [UxmlAttribute("codegen-command")]
    public string CommandName { get; set; } = string.Empty;

}
