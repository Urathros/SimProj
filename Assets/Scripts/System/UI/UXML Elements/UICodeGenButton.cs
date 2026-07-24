using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("Button", libraryPath = "Codegen")]
public partial class UICodeGenButton : Button, IUICodeGenClickable, IUICodeGenCommandSource
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;

    [UxmlAttribute("codegen-command")]
    public string CommandName { get; set; } = string.Empty;
}
