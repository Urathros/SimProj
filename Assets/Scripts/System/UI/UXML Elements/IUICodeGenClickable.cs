using UnityEngine;
using UnityEngine.UIElements;

public interface IUICodeGenClickable
{
    [UxmlAttribute("codegen-click")]
    public string CodeGenClick { get; set; } 

}
