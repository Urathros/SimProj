using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("MultiColumnTreeView", libraryPath = "Codegen")]
public partial class UICodeGenMultiColumnTreeView : MultiColumnTreeView, IUICodeGenItemsSource, IUICodeGenSelectionBindable, IUICodeGenCommandSource
{
    [UxmlAttribute("codegen-bind-source")]
    public string ItemsSourceBinding { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-selected-item")]
    public string SelectedItemBinding { get; set; } = string.Empty;

    [UxmlAttribute("codegen-bind-selected-index")]
    public string SelectedIndexBinding { get; set; } = string.Empty;

    [UxmlAttribute("codegen-command")]
    public string CommandName { get; set; } = string.Empty;
}
