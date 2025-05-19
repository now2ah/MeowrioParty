using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using System.Collections.Generic;

public enum CameraType
{
    None,
    Board,
    PlayerFocus,
    PlayerMove,
}

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineCamera _currentLiveCamera;
    [SerializeField] private float _fov = 60f;

    [SerializeField] private CinemachineCamera _boardCamera;
    [SerializeField] private Vector3 _boardCameraStartPosition;
    [SerializeField] private Vector3 _boardCameraStartRotation;

    [SerializeField] private CinemachineCamera _playerFocusCamera;
    [SerializeField] private Vector3 _playerFocusCameraStartPosition;
    [SerializeField] private Vector3 _playerFocusCameraStartRotation;

    [SerializeField] private CinemachineCamera _playerMoveCamera;
    [SerializeField] private Vector3 _playerMoveCameraStartPosition;
    [SerializeField] private Vector3 _playerMoveCameraStartRotation;

    [SerializeField] private List<CinemachineCamera> _virtualCameraList;

    private Camera _mainCamera;
    private CinemachineBrain _cinemachineBrain;
    private GameObject _focusObject;
    private CinemachineFollow _playerMoveFollow;
    private CinemachineHardLookAt _playerMoveHardLookAt;
    private CinemachineFollow _playerFocusFollow;
    private CinemachineHardLookAt _playerFocusHardLookAt;

    public override void Awake()
    {
        base.Awake();
        _boardCameraStartPosition = new Vector3(0f, 6f, -6f);
        _boardCameraStartRotation = new Vector3(55f, 0f, 0f);
        _playerFocusCameraStartPosition = new Vector3(0f, 2f, -9f);
        _playerFocusCameraStartRotation = new Vector3(0f, 0f, 0f);
        _playerMoveCameraStartPosition = new Vector3(-0.5f, 4f, -9f);
        _playerMoveCameraStartRotation = new Vector3(40f, -30f, 0f);

        _mainCamera = transform.GetChild(0).GetComponent<Camera>();
        _mainCamera.tag = "MainCamera";
        _cinemachineBrain = _mainCamera.AddComponent<CinemachineBrain>();
        _focusObject = new GameObject("CameraFocus");
        _focusObject.transform.SetParent(transform);
        _focusObject.transform.position = Vector3.zero;
        if (null == _virtualCameraList)
            _virtualCameraList = new List<CinemachineCamera>();
        InitializeVirtualCameras();
        _boardCamera.Prioritize();
        _currentLiveCamera = _boardCamera;
    }

    private void InitializeVirtualCameras()
    {
        if (_boardCamera == null)
        {
            GameObject boardCameraObj = new GameObject("BoardCamera");
            boardCameraObj.transform.SetParent(transform);
            _boardCamera = boardCameraObj.AddComponent<CinemachineCamera>();
            _boardCamera.Lens.FieldOfView = _fov;
            boardCameraObj.transform.position = _boardCameraStartPosition;
            boardCameraObj.transform.rotation = Quaternion.Euler(_boardCameraStartRotation);
        }

        if (_playerFocusCamera == null)
        {
            GameObject playerFocusCameraObj = new GameObject("PlayerFocusCamera");
            playerFocusCameraObj.transform.SetParent(transform);
            _playerFocusCamera = playerFocusCameraObj.AddComponent<CinemachineCamera>();
            _playerFocusCamera.Lens.FieldOfView = _fov;
            playerFocusCameraObj.transform.position = _playerFocusCameraStartPosition;
            playerFocusCameraObj.transform.rotation = Quaternion.Euler(_playerFocusCameraStartRotation);
            _playerFocusFollow = playerFocusCameraObj.AddComponent<CinemachineFollow>();
            _playerFocusFollow.FollowOffset = new Vector3(0f, 1f, -5f);
            _playerFocusHardLookAt = playerFocusCameraObj.AddComponent<CinemachineHardLookAt>();
        }

        if (_playerMoveCamera == null)
        {
            GameObject playerMoveCameraObj = new GameObject("PlayerMoveCamera");
            playerMoveCameraObj.transform.SetParent(transform);
            _playerMoveCamera = playerMoveCameraObj.AddComponent<CinemachineCamera>();
            _playerMoveCamera.Lens.FieldOfView = _fov;
            playerMoveCameraObj.transform.position = _playerMoveCameraStartPosition;
            playerMoveCameraObj.transform.rotation = Quaternion.Euler(_playerMoveCameraStartRotation);
            _playerMoveFollow = playerMoveCameraObj.AddComponent<CinemachineFollow>();
            _playerMoveFollow.FollowOffset = new Vector3(3f, 3f, -5f);
            _playerMoveHardLookAt = playerMoveCameraObj.AddComponent<CinemachineHardLookAt>();
        }
    }

    public void Focus(GameObject gameObject, CameraType type)
    {
        ChangeCamera(type);
        SetTarget(gameObject.transform);
    }

    public void SetTarget(Transform targetTransform)
    {
        if (_focusObject != null)
        {
            _focusObject.transform.SetParent(targetTransform, false);
            _currentLiveCamera.Target.TrackingTarget = _focusObject.transform;
        }
    }

    public void ChangeCamera(CameraType type)
    {
        switch (type)
        {
            case CameraType.Board:
                _boardCamera.Prioritize();
                _currentLiveCamera = _boardCamera;
                break;

            case CameraType.PlayerFocus:
                _playerFocusCamera.Prioritize();
                _currentLiveCamera = _playerFocusCamera;
                break;

            case CameraType.PlayerMove:
                _playerMoveCamera.Prioritize();
                _currentLiveCamera = _playerMoveCamera;
                break;
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
