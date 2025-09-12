using Meowrio.Manager;
using Unity.Netcode;
using UnityEngine;

namespace Meowrio.Util
{
    public class InputHandler : NetworkBehaviour
    {
        [SerializeField] private InputManagerSO _inputManager;

        public void OnEnable()
        {
            _inputManager.OnConfirmButtonCanceled += InputManager_OnConfirmButtonCanceled;
        }

        public void OnDisable()
        {
            _inputManager.OnConfirmButtonCanceled -= InputManager_OnConfirmButtonCanceled;
        }

        private void InputManager_OnConfirmButtonCanceled(object sender, bool e)
        {
            RequestInputServerRpc((int)NetworkManager.Singleton.LocalClientId);
        }

        [Rpc(SendTo.Server)]
        private void RequestInputServerRpc(int clientId)
        {
            BoardGameManager.Instance.ProcessInput(clientId);
        }
    }
}
