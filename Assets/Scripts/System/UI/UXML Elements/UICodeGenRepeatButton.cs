using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("RepeatButton", libraryPath = "Codegen")]
public partial class UICodeGenRepeatButton : RepeatButton, IUICodeGenClickable, IUICodeGenCommandSource
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } = string.Empty;

    [UxmlAttribute("codegen-command")]
    public string CommandName { get; set; } = string.Empty;

}
