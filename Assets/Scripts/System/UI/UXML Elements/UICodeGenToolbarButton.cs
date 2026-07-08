using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("Button", libraryPath = "Codegen")]
public partial class UICodeGenToolbarButton : ToolbarButton, IUICodeGenClickable
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;
}
