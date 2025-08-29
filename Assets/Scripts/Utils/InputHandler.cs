using Meowrio.Manager;
using Unity.Netcode;

namespace Meowrio.Util
{
    public class InputHandler : NetworkBehaviour
    {
        private BoardGameManager _boardGameManager;
        private InputManagerSO _inputManager;

        public override void OnNetworkSpawn()
        {
            _inputManager.OnConfirmButtonPerformed += InputManager_OnConfirmButtonPerformed;
        }

        private void InputManager_OnConfirmButtonPerformed(object sender, bool e)
        {
            RequestInputServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void RequestInputServerRpc()
        {
            _boardGameManager.ProcessInput((int)OwnerClientId);
        }
    }
}
