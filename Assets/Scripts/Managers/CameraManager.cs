using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using System.Collections.Generic;
using Unity.Netcode;

public enum CameraType
{
    Board,
    Stage,
    Focus,
    Move,
}

public class CameraManager : Singleton<CameraManager>
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

    [Rpc(SendTo.Everyone)]
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
