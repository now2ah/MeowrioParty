using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputManager", menuName = "ScriptableObject/InputManagerSO")]
public class InputManagerSO : ScriptableObject
{
    [SerializeField] private InputActionAsset _inputActionAsset;

    private InputAction _confirmAction;

    public event EventHandler<bool> OnConfirmButtonStarted;
    public event EventHandler<bool> OnConfirmButtonPerformed;
    public event EventHandler<bool> OnConfirmButtonCancelded;

    private void OnEnable()
    {
        if (null == _inputActionAsset)
            return;

        _confirmAction = _inputActionAsset.FindAction("Confirm");

        _confirmAction.started += OnConfirmAction_started;
        _confirmAction.performed += OnConfirmAction_performed;
        _confirmAction.canceled += OnConfirmAction_canceled;

        _confirmAction.Enable();
    }

    private void OnDisable()
    {
        _confirmAction.started -= OnConfirmAction_started;
        _confirmAction.performed -= OnConfirmAction_performed;
        _confirmAction.canceled -= OnConfirmAction_canceled;

        _confirmAction.Disable();
    }

    private void OnConfirmAction_started(InputAction.CallbackContext context)
    {
        OnConfirmButtonStarted?.Invoke(this, context.ReadValueAsButton());
    }

    private void OnConfirmAction_performed(InputAction.CallbackContext context)
    {
        OnConfirmButtonPerformed?.Invoke(this, context.ReadValueAsButton());
    }

    private void OnConfirmAction_canceled(InputAction.CallbackContext context)
    {
        OnConfirmButtonCancelded?.Invoke(this, context.ReadValueAsButton());
    }

}
