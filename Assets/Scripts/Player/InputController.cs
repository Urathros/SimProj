using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class InputController : MonoBehaviour, IReflectable
{

    private PlayerInputActions _input = null;

    public event Action<CallbackContext> MovementAction
    {
        add
        {
            if (_input == null) return;

            _input.Topdown.Movement.performed += value;
            _input.Topdown.Movement.canceled += value;
        }
        remove
        {
            if (_input == null) return;

            _input.Topdown.Movement.canceled -= value;
            _input.Topdown.Movement.performed -= value;
        }
    }



    public event Action<CallbackContext> MovementStartAction
    {
        add
        {
            if (_input == null) return;

            _input.Topdown.Movement2.started += value;
        }
        remove
        {
            if (_input == null) return;

            _input.Topdown.Movement2.started -= value;
        }
    }

    public event Action<CallbackContext> MovementPerformAction
    {
        add
        {
            if (_input == null) return;

            _input.Topdown.Movement2.performed += value;
        }
        remove
        {
            if (_input == null) return;

            _input.Topdown.Movement2.performed -= value;
        }
    }

    public event Action<CallbackContext> MovementCancelAction
    {
        add
        {
            if (_input == null) return;

            _input.Topdown.Movement2.canceled += value;
        }
        remove
        {
            if (_input == null) return;

            _input.Topdown.Movement2.canceled -= value;
        }
    }



    private void Awake()
    {
        if (_input == null) _input = new();
    }

    private void OnEnable()
    {
        if (_input != null) _input.Enable();
    }

    private void OnDisable()
    {
        if (_input != null) _input.Disable();
    }

}
