using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using System.Collections.Generic;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineCamera _currentLiveCamera;

    [SerializeField] private List<CinemachineCamera> _virtualCameraList;

    private Camera _mainCamera;
    private CinemachineBrain _cinemachineBrain;
    private GameObject _focusObject;

    public override void Awake()
    {
        base.Awake();

        _mainCamera = transform.GetChild(0).GetComponent<Camera>();
        _mainCamera.tag = "MainCamera";
        _cinemachineBrain = _mainCamera.AddComponent<CinemachineBrain>();
        _focusObject = new GameObject("CameraFocus");
        _focusObject.transform.SetParent(transform);
        _focusObject.transform.position = Vector3.zero;
        if (null == _virtualCameraList)
            _virtualCameraList = new List<CinemachineCamera>();
    }

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
