using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[DefaultExecutionOrder(-10_000)]
public class SceneInjector : Singleton<SceneInjector>
{
    private const string SceneRootGOName = "[Scene Dependencies]";
    private const BindingFlags InjectionFlags =   BindingFlags.Instance
                                                | BindingFlags.Public
                                                | BindingFlags.NonPublic;

    [SerializeField]
    private Transform _rootContainer = null;

    private readonly List<Component> _components = new();
    private readonly List<object> _services = new();
    private readonly Dictionary<Type, object> _registry = new();

    public IReadOnlyList<Component> Components => _components;
    public IReadOnlyList<object> Services => _services;

    private void ValidateContainerRoot()
    {
        if (_rootContainer != null) return;

        var rootGO = new GameObject(SceneRootGOName);
        rootGO.transform.SetParent(transform, false);
        _rootContainer = rootGO.transform;
    }

    private static bool IsInjectableProperty(PropertyInfo property)
    {
        return    property.CanWrite 
               && typeof(IReflectable).IsAssignableFrom(property.PropertyType);
    }

    private Component CreateUnityComponent(Type componentType)
    {
        if(!typeof(Component).IsAssignableFrom(componentType))
        {
            throw new ArgumentException(
                $"{componentType.Name} is not a Unity Component.",
                nameof(componentType));
        }

        var go = new GameObject(componentType.Name);
        go.transform.SetParent(_rootContainer, false);
        var comp = go.AddComponent(componentType);
        _components.Add(comp);

        return comp;
    }

    private object CreateManagedService(Type serviceType)
    {

        if (typeof(UnityEngine.Object).IsAssignableFrom(serviceType))
        {
            throw new InvalidOperationException(
                $"UnityEngine.Object type {serviceType.Name} must be created through Unity.");
        }

        var instance = Activator.CreateInstance(serviceType)
            ?? throw new InvalidOperationException($"Could not create Instance of {serviceType.Name}");

        _services.Add(instance);

        return instance;
    }

    private void RegisterProperties()
    {
        var properties = GetType()
                         .GetProperties(InjectionFlags)
                         .Where(IsInjectableProperty).ToArray();

        foreach (var property in properties)
        {
            var dependencyType = property.PropertyType;

            var instance = typeof(Component).IsAssignableFrom(dependencyType)
                              ? CreateUnityComponent(dependencyType)
                              : CreateManagedService(dependencyType);

            property.SetValue(this, instance);
            _registry[dependencyType] = instance;
        }
    }

    private void InjectInto(object target)
    {
        var targetType = target.GetType();

        foreach (var property in targetType.GetProperties(InjectionFlags))
        {
            if (!property.CanWrite) continue;

            if (!property.IsDefined(typeof(InjectAttribute), inherit: true)) continue;



            if (_registry.TryGetValue(property.PropertyType, out object dependency))
            {
                property.SetValue(target, dependency);
                continue;
            }

            Debug.LogError(
                $"Missing dependency {property.PropertyType.Name} for {targetType.Name}.{property.Name}",
                this);
        }
    }

    private void InjectDependencies()
    {
        foreach (var target in _registry.Values)
        {
            InjectInto(target);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        ValidateContainerRoot();
        RegisterProperties();
        InjectDependencies();
    }
}
