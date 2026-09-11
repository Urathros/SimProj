using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class UICodeGenBindingFactory
{
    public IUICodeGenBinding BindValue<TSource, TValue>( TSource source,
                                                         Func<TSource, TValue> getter,
                                                         Action<TSource, TValue> setter,
                                                         BaseField<TValue> target,
                                                         UICodeGenBindMode mode)
        where TSource : class
    => new BaseFieldBinding<TSource, TValue>(source, getter, setter, target, mode);
}
