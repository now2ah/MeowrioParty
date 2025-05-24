using System;
using UnityEngine;

// TODO: 보드 매니저에게 결과 전송과 보상 처리하라고 보낼려면 미니 게임 상태를 관리하고 있는 미니 게임 매니저가 필요할거 같음
// 미니 게임 매니저로 보드 매니저가 하던거 처럼 ready상태와 end 상태일 때 입력을 막고 연출을 보여줘야 함

public class MiniGameManager : MonoBehaviour
{
    private float _finishLineX;
    private float _baseSpeed;
    private float _maxSpeed;
    private float _acceleration;
    private float _deceleration;

    [SerializeField] private float _currentSpeed;

    [SerializeField] private Transform _miniGameStartPos;
    private Vector3 _originalPos;

    [SerializeField] private KeyCode _inputKey = KeyCode.A;

    public bool isMiniFinished = false;

    private Animator _animator;
    private float _defaultAnimSpeed;

    private void Awake()
    {
        _finishLineX = 30.0f;
        _baseSpeed = 1.0f;
        _maxSpeed = 10.0f;
        _acceleration = 3.0f;
        _deceleration = 5.0f;
        _currentSpeed = 0.0f;

        _animator = GetComponent<Animator>();
        _defaultAnimSpeed = _animator.speed;

        _originalPos = transform.position;

        if (_miniGameStartPos != null )
        {
            transform.position = _miniGameStartPos.position;
            transform.rotation = _miniGameStartPos.rotation;
        }        
    }
    private void Start()
    {
        _animator.SetBool("isMoving", true);
    }

    private void Update()
    {
        if (isMiniFinished)
        {
            return;
        }
        PressedButton();
        Deceleration();
        Move();
        CheckFinish();
    } 

    private void PressedButton()
    {        
        if (Input.GetKeyDown(_inputKey))
        {
            Accelerate();
        }
    }

    private void Accelerate()
    {
        _currentSpeed += _acceleration;
        _currentSpeed = Mathf.Clamp(_currentSpeed, _baseSpeed, _maxSpeed);
    }

    private void Deceleration()
    {
        _currentSpeed -= _deceleration * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, _baseSpeed, _maxSpeed);
    }

    private void Move()
    {
        // 플레이어의 보여지는 부분
        transform.Translate(Vector3.forward * _currentSpeed * Time.deltaTime);
        UpdateAnimatorSpeed();
    }

    private void UpdateAnimatorSpeed()
    {
        float speedRatio = _currentSpeed / _baseSpeed;
        _animator.speed = Mathf.Clamp(speedRatio, 0.5f, 2.0f);
    }

    private void CheckFinish()
    {
        if (transform.position.x >= _finishLineX)
        {
            isMiniFinished = true;
            _currentSpeed = 0.0f;
            Debug.Log(gameObject.name + "결승선 도착");
            // 애니메이션 연출
            _animator.SetBool("isMoving", false);
            _animator.speed = _defaultAnimSpeed;
            // 미니게임 매니저로 게임 끝났다는걸 알림
            ReturnToOriginalPos();
        }
    }

    private void ReturnToOriginalPos()
    {
        transform.position = _originalPos;
        transform.rotation = Quaternion.identity;
        Debug.Log("원래 위치 복귀");
    }
}
