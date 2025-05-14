using UnityEngine;

public class InputManagerTest : MonoBehaviour
{
    public InputManagerSO inputManager;

    void Start()
    {
        inputManager.OnConfirmButtonStarted += InputManager_OnConfirmButtonStarted;
        inputManager.OnConfirmButtonPerformed += InputManager_OnConfirmButtonPerformed;
        inputManager.OnConfirmButtonCancelded += InputManager_OnConfirmButtonCancelded;
    }

    private void InputManager_OnConfirmButtonCancelded(object sender, bool e)
    {
        Debug.Log("canceled");
    }

    private void InputManager_OnConfirmButtonPerformed(object sender, bool e)
    {
        Debug.Log("performed");
    }

    private void InputManager_OnConfirmButtonStarted(object sender, bool e)
    {
        Debug.Log("started");
    }
}
