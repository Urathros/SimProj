using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("SliderInt", libraryPath = "Codegen")]
public partial class UICodeGenSliderInt : SliderInt, IUICodeGenBindable
{
    [UxmlAttribute("codegen-bind-name")]
    public string BindName { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-mode")]
    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;

    public string BindingTargetProperty => nameof(value);
}