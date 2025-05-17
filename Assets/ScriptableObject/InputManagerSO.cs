using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputManager", menuName = "ScriptableObject/InputManagerSO")]
public class InputManagerSO : ScriptableObject
{
    [SerializeField] private InputActionAsset _inputActionAsset;

    private InputAction _confirmAction;

    private InputAction _player0DiceAction;
    private InputAction _player1DiceAction;

    public event EventHandler<bool> OnConfirmButtonStarted;
    public event EventHandler<bool> OnConfirmButtonPerformed;
    public event EventHandler<bool> OnConfirmButtonCancelded;

    //temporary code for milestone 1
    public event EventHandler<bool> OnPlayer0DiceButtonPerformed;
    public event EventHandler<bool> OnPlayer1DiceButtonPerformed;

    private void OnEnable()
    {
        if (null == _inputActionAsset)
            return;

        _confirmAction = _inputActionAsset.FindAction("Confirm");

        _confirmAction.started += OnConfirmAction_started;
        _confirmAction.performed += OnConfirmAction_performed;
        _confirmAction.canceled += OnConfirmAction_canceled;

        _confirmAction.Enable();

        //temporary for milestone 1
        _player0DiceAction = _inputActionAsset.FindAction("Player0Dice");
        _player1DiceAction = _inputActionAsset.FindAction("Player1Dice");
        _player0DiceAction.performed += _player0DiceAction_performed;
        _player1DiceAction.performed += _player1DiceAction_performed;
        _player0DiceAction.Enable();
        _player1DiceAction.Enable();
    }

    private void OnDisable()
    {
        _confirmAction.started -= OnConfirmAction_started;
        _confirmAction.performed -= OnConfirmAction_performed;
        _confirmAction.canceled -= OnConfirmAction_canceled;

        _confirmAction.Disable();

        //temporary for milestone 1
        _player0DiceAction.performed -= _player0DiceAction_performed;
        _player1DiceAction.performed -= _player1DiceAction_performed;
        _player0DiceAction.Disable();
        _player1DiceAction.Disable();
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
    private void _player0DiceAction_performed(InputAction.CallbackContext context)
    {
        OnPlayer0DiceButtonPerformed?.Invoke(this, context.ReadValueAsButton());
    }

    private void _player1DiceAction_performed(InputAction.CallbackContext context)
    {
        OnPlayer1DiceButtonPerformed?.Invoke(this, context.ReadValueAsButton());
    }
}
