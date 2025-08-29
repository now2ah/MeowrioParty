
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Meowrio.Controller
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameObject _focusObject;

        [SerializeField] private CinemachineCamera _currentLiveCamera;

        [SerializeField] private List<CinemachineCamera> _virtualCameraList;

        private Camera _mainCamera;
        private CinemachineBrain _cinemachineBrain;

        public void SetTarget(Transform targetTransform)
        {
            if (_focusObject != null)
            {
                _focusObject.transform.SetParent(targetTransform, false);
                _currentLiveCamera.Target.TrackingTarget = _focusObject.transform;
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
