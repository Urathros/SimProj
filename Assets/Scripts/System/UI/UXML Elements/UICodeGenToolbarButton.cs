using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("ToolbarButton", libraryPath = "Codegen")]
public partial class UICodeGenToolbarButton : ToolbarButton, IUICodeGenClickable, IUICodeGenCommandSource
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;


    [UxmlAttribute("codegen-command")]
    public string CommandName { get; set; } = string.Empty;
}
