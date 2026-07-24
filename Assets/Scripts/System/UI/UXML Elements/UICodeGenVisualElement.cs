using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("VisualElement", libraryPath = "Codegen")]
public partial class UICodeGenVisualElement : VisualElement, IUICodeGenClickable
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;
}
