using UnityEngine;

/*
 TODO:
UICodeGenObjectField
UICodeGenColorField
UICodeGenLayerField
UICodeGenLayerMaskField
UICodeGenTagField
UICodeGenMaskField
UICodeGenEnumField
UICodeGenEnumFlagsField
UICodeGenVector2Field
UICodeGenVector3Field
UICodeGenVector4Field
UICodeGenVector2IntField
UICodeGenVector3IntField
UICodeGenRectField
UICodeGenRectIntField
UICodeGenBoundsField
UICodeGenBoundsIntField
UICodeGenCurveField
UICodeGenGradientField
 */
public interface IUICodeGenBindable
{
    string BindName { get; }
    UICodeGenBindMode BindMode { get; }
    string BindingTargetProperty { get; }
}
