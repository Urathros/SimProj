using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseFieldBinding<TSource, TValue> : IUICodeGenBinding where TSource : class
{
    private readonly TSource _source = default;
    private readonly Func<TSource, TValue> _getter = source => default;
    private readonly Action<TSource, TValue> _setter = (source, val) => { };
    private readonly BaseField<TValue> _target = null;
    private readonly UICodeGenBindMode _mode = UICodeGenBindMode.None;


    private bool _isUpdating = false;

    public BaseFieldBinding( TSource source, 
                             Func<TSource, TValue> getter, 
                             Action<TSource, TValue> setter,
                             BaseField<TValue> target,
                             UICodeGenBindMode mode)
    {
        _source = source;
        _getter = getter;
        _setter = setter;
        _target = target;
        _mode = mode;

        if (_mode is UICodeGenBindMode.OneWay
            or UICodeGenBindMode.TwoWay
            or UICodeGenBindMode.OneTime) RefreshTarget();

        if (_mode is UICodeGenBindMode.TwoWay) _target.RegisterValueChangedCallback(HandleTargetValueChanged);
    }

    private void HandleTargetValueChanged(ChangeEvent<TValue> evt)
    {
        if(_isUpdating || _setter == null) return;
        _setter(_source, evt.newValue);
    }

    public void Dispose() => _target.UnregisterValueChangedCallback(HandleTargetValueChanged);

    public void RefreshSource()
    {
        if (_isUpdating || _setter == null) return;

        try
        {
            _isUpdating = true;
            _setter(_source, _target.value);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    public void RefreshTarget()
    {
        if (_isUpdating) return;

        try
        {
            _isUpdating = true;
            _target.SetValueWithoutNotify(_getter(_source));
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        finally
        {
            _isUpdating = false;
        }
    }
}
