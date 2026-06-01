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
