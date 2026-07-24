using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

[UxmlElement("TextElement", libraryPath = "Codegen")]
public partial class UICodeGenTextElement : TextElement, IUICodeGenBindable
{
    [UxmlAttribute("codegen-bind-name")]
    public string BindName { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-mode")]
    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;

    public string BindingTargetProperty => nameof(text);
}
