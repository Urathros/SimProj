using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("Button", libraryPath = "Codegen")]
public partial class UICodeGenButton : Button, IUICodeGenClickable
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;
}
