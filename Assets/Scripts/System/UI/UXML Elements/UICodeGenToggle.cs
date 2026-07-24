using UnityEngine.UIElements;

[UxmlElement("Toggle", libraryPath = "Codegen")]
public partial class UICodeGenToggle : Toggle, IUICodeGenBindable, IUICodeGenCommandSource
{
    [UxmlAttribute("codegen-bind-name")]
    public string BindName { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-mode")]
    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;


    public string BindingTargetProperty => nameof(value);

    [UxmlAttribute("codegen-command")]
    public string CommandName { get; set; } = string.Empty;
}
