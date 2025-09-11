using Meowrio.Manager;
using Unity.Netcode;
using UnityEngine;

namespace Meowrio.Util
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private InputManagerSO _inputManager;

        public void OnEnable()
        {
            _inputManager.OnConfirmButtonPerformed += InputManager_OnConfirmButtonPerformed;
        }

        public void OnDisable()
        {
            _inputManager.OnConfirmButtonPerformed -= InputManager_OnConfirmButtonPerformed;
        }

        private void InputManager_OnConfirmButtonPerformed(object sender, bool e)
        {
            RequestInputServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void RequestInputServerRpc()
        {
            BoardGameManager.Instance.ProcessInput((int)NetworkManager.Singleton.LocalClientId);
        }
    }
}
