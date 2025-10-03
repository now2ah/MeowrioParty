
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Meowrio.Controller
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private GameObject _focusObject;

        [SerializeField] private CinemachineCamera _currentLiveCamera;

        [SerializeField] private List<CinemachineCamera> _virtualCameraList;

        private Camera _mainCamera;
        private CinemachineBrain _cinemachineBrain;

        [Rpc(SendTo.ClientsAndHost)]
        public void SetTargetRpc(ulong currentTurnNetworkObjKey)
        {
            if (_focusObject != null)
            {
                if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(currentTurnNetworkObjKey, out NetworkObject networkObj))
                {
                    if (networkObj.TryGetComponent<PlayerController>(out PlayerController playerController))
                    {
                        _focusObject.transform.SetParent(playerController.transform, false);
                        _currentLiveCamera.Target.TrackingTarget = _focusObject.transform;
                    }
                }
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ChangeCameraRpc(CameraType type)
        {
            if (_virtualCameraList != null && _virtualCameraList.Count > 0)
            {
                int index = (int)type;
                _virtualCameraList[index].Prioritize();
                _currentLiveCamera = _virtualCameraList[index];
            }
        }
    }
}
