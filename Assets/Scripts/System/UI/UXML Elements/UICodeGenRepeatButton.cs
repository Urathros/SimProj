using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("Button", libraryPath = "Codegen")]
public partial class UICodeGenRepeatButton : RepeatButton, IUICodeGenClickable
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;

}
