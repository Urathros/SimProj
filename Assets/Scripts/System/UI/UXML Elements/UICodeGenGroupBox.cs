using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("GroupBox", libraryPath = "Codegen")]
public partial class UICodeGenGroupBox : GroupBox, IUICodeGenClickable
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;
}
