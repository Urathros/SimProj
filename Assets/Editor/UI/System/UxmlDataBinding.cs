using System;
using UnityEngine;

public sealed class UxmlDataBinding
{
    public string ElementName { get; set; } = string.Empty;

    public Type ElementType { get; set; } = null;

    public string SourcePath { get; set; } = string.Empty;

    public string TargetProperty { get; set; } = string.Empty;

    public UICodeGenBindMode BindMode { get; set; } = UICodeGenBindMode.None;

    public UICodeGenBindKind BindKind { get; set; } = UICodeGenBindKind.None;
}
