using System;
using System.Reflection;
using UnityEngine;

public static class SingletonInjector
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static void Inject(object target)
    {
        if (target == null) return;

        var fields = target.GetType().GetFields(Flags);

        foreach (var field in fields)
        {
            if (!Attribute.IsDefined(field, typeof(SingletonPropertyAttribute))) continue;

            var fieldType = field.FieldType;

            var instanceProperty = fieldType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            
            if(instanceProperty == null)
            {
                Debug.LogError(
                    $"[{target.GetType().Name}] {fieldType.Name} has no static Instance property.");

                continue;
            }

            var value = instanceProperty.GetValue(null);

            if(value == null)
            {
                Debug.LogError(
                    $"[{target.GetType().Name}] Failed to resolve singleton {fieldType.Name}.");

                continue;
            }


            field.SetValue(target, value);
        }
    }
}
