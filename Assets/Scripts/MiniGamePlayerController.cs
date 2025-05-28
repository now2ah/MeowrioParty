using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class MiniGamePlayerController : NetworkBehaviour
{
    public InputManagerSO inputManager;

    public event Action OnFinish;

    private float _baseSpeed;
    private float _maxSpeed;
    private float _acceleration;
    private float _deceleration;
    private float _currentSpeed;

    private Vector3 _finishPos;

    private NetworkVariable<int> _speedStage = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Animator _animator;
    private float _defaultAnimSpeed;

    private bool _isReady = false;
    private bool _isMiniFinished = false;

    private static readonly float[] _animSpeeds = { 0.5f, 1.0f, 1.5f, 2.0f };

    private void Awake()
    {
        _baseSpeed = 1.0f;
        _maxSpeed = 10.0f;
        _acceleration = 3.0f;
        _deceleration = 5.0f;
        _currentSpeed = 0.0f;
    }

    public override void OnNetworkSpawn()
    {
        _animator = GetComponent<Animator>();
        _defaultAnimSpeed = _animator.speed;
        _animator.SetBool("isMoving", true);

        _speedStage.OnValueChanged += OnSpeedStageChanged;

        if (IsOwner)
        {
            StartCoroutine(WaitUntilSceneReady());
            inputManager.OnConfirmButtonPerformed += GetInput;
        }
    }
    public override void OnNetworkDespawn()
    {
        Debug.Log("OnNetworkDespawn");
        _speedStage.OnValueChanged -= OnSpeedStageChanged;

        if (IsOwner && inputManager != null)
        {
            inputManager.OnConfirmButtonPerformed -= GetInput;
        }
    }

    private IEnumerator WaitUntilSceneReady()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "TapRaceScene");

        if (!IsOwner) yield break;
    }

    private void Start()
    {
        _isReady = true;
    }

    private void Update()
    {
        if (!_isReady || _isMiniFinished) return;

        Decelerate();
        Move();
        CheckFinish();
        if (!IsOwner)
        {
            UpdateAnimatorSpeed(_speedStage.Value);
        }
    }

    public void SetStartPosition(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    private void GetInput(object sender, bool isPressed)
    {
        if (!isPressed || !IsOwner) return;

        AccelerateRpc();
    }

    [Rpc(SendTo.Server)]
    private void AccelerateRpc()
    {
        _currentSpeed += _acceleration;
        _currentSpeed = Mathf.Clamp(_currentSpeed, _baseSpeed, _maxSpeed);

        UpdateSpeedStage();
    }

    private void Decelerate()
    {
        _currentSpeed -= _deceleration * Time.deltaTime;
        _currentSpeed = Mathf.Max(_currentSpeed, _baseSpeed);
        if (IsServer)
        {
            UpdateSpeedStage();
        }
    }

    private void Move()
    {
        transform.Translate(Vector3.forward * _currentSpeed * Time.deltaTime);
    }

    private void UpdateSpeedStage()
    {
        int stage = Mathf.FloorToInt((_currentSpeed / _maxSpeed) * (_animSpeeds.Length - 1));
        stage = Mathf.Clamp(stage, 0, _animSpeeds.Length - 1);

        if (_speedStage.Value != stage)
        {
            _speedStage.Value = stage;
        }
    }
   
    private void OnSpeedStageChanged(int oldStage, int newStage)
    {
        UpdateAnimatorSpeed(newStage); 
    }

    private void UpdateAnimatorSpeed(int stage)
    {
        int clampedStage = Mathf.Clamp(stage, 0, _animSpeeds.Length - 1);
        _animator.speed = _animSpeeds[clampedStage];
    }

    public void SetFinishPosition(Vector3 finishPos)
    {
        _finishPos = finishPos;
    }

    private void CheckFinish()
    {
        float distanceX = Mathf.Abs(transform.position.x - _finishPos.x);

        if (distanceX <= 0.5f && !_isMiniFinished)
        {
            _isMiniFinished = true;
            _currentSpeed = 0f;
            _animator.SetBool("isMoving", false);
            _animator.speed = _defaultAnimSpeed;

            Debug.Log(gameObject.name + " °á½Â¼± µµÂø");

            // ¼­¹ö¿¡°Ô ¾Ë¸²
            NotifyServerRaceFinishedServerRpc();
            OnFinish?.Invoke();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void NotifyServerRaceFinishedServerRpc()
    {
        LeaderBoardManager.Instance.UpdateCoin(OwnerClientId, 1);
        LeaderBoardManager.Instance.UpdateLeaderBoardClient();
        
        BoardManager.Instance.OnMiniGamePlayerFinished(OwnerClientId);
    }
}