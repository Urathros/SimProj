using UnityEngine.UIElements;

[UxmlElement("Scroller", libraryPath = "Codegen")]
public partial class UICodeGenScroller : Scroller, IUICodeGenBindable
{
    [UxmlAttribute("codegen-bind-name")]
    public string BindName { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-mode")]
    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;

    public string BindingTargetProperty => nameof(value);
}
