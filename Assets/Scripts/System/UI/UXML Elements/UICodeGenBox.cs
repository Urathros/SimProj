using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("Box", libraryPath = "Codegen")]
public partial class UICodeGenBox : Box, IUICodeGenClickable
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;
}
