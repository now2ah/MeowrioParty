using System;
using System.Collections.Generic;
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

    public event Action<int> OnDiceButtonPerformed;
    private Dictionary<int, InputAction> _playerDiceActions = new(); //일단 플레이어별로 인풋액션 만들고 서버 붙이면 단일 Action으로 수정
    

    private void OnEnable()
    {
        if (null == _inputActionAsset)
            return;

        _confirmAction = _inputActionAsset.FindAction("Confirm");

        _confirmAction.started += OnConfirmAction_started;
        _confirmAction.performed += OnConfirmAction_performed;
        _confirmAction.canceled += OnConfirmAction_canceled;

        _confirmAction.Enable();

        for (int i = 0; i < 2; i++)
        {
            int capturedID = i;
            var action = _inputActionAsset.FindAction($"Player{capturedID}Dice");
            if (action != null)
            {
                action.performed += ctx => OnDiceButtonPerformed?.Invoke(capturedID);
                action.Enable();
                _playerDiceActions[capturedID] = action;
            }
        }
    }

    private void OnDisable()
    {
        _confirmAction.started -= OnConfirmAction_started;
        _confirmAction.performed -= OnConfirmAction_performed;
        _confirmAction.canceled -= OnConfirmAction_canceled;

        _confirmAction.Disable();

        foreach (var playerAction in _playerDiceActions)
        {
            playerAction.Value.Disable();
            playerAction.Value.performed -= ctx => OnDiceButtonPerformed?.Invoke(playerAction.Key);
        }
        _playerDiceActions.Clear(); // 일단 비우는 함수 작성해줌
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
