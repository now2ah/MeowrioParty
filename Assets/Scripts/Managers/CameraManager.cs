using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using System.Collections.Generic;

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

    public void ChangeCamera(int index)
    {
        if (_virtualCameraList != null && _virtualCameraList.Count > 0)
        {
            _virtualCameraList[index].Prioritize();
            _currentLiveCamera = _virtualCameraList[index];
        }
    }
}
