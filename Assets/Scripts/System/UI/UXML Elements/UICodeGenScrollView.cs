using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("ScrollView", libraryPath = "Codegen")]
public partial class UICodeGenScrollView : ScrollView, IUICodeGenClickable
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;
}
