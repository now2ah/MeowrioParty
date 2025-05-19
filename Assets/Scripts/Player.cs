using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
// COMMENT : 플레이어 이동과 관련된 로직만 처리하면 됨
// COMMENT : BoardManager가 가야할 Tile을 알려줌

public class Player : MonoBehaviour
{
    public int playerID; //일단 public으로선언언
    [SerializeField] private InputManagerSO _inputManager;

    public Tile currentTile; // COMMENT: 게임 진행과 관련된 부분이므로 BoardManager가 갖고 있어야 함.

    [SerializeField] public Dice _dice;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>(); // Comment: 아래 변수도 불필요

    private bool _isMoving = false;
    [SerializeField] private float moveSpeed = 3f;

    public bool IsMoving { get { return _isMoving; } }

    private Animator _animator;

    private void OnEnable()
    {
        _inputManager.OnDiceButtonPerformed += OnDiceInputReceived;
    }
    private void OnDisable()
    {
        _inputManager.OnDiceButtonPerformed -= OnDiceInputReceived;
    }

    void Start()
    {
        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(200, 50);
        _animator = GetComponent<Animator>();
    }
    public int RollDice()
    {
        _dice.gameObject.SetActive(false);
        _animator.SetTrigger("Jump");
        return _dice.Roll();
    }

    public void MoveTo(Tile nextTile)
    {
        if (!_isMoving)
        {
            _isMoving = true;
            _animator.SetBool("isMoving", true);
            Vector3 endPos = nextTile.transform.position;
            Vector3 direction = (endPos - transform.position).normalized;
            direction.y = 0f;
            transform.DOLookAt(endPos, 0.2f);

            transform.DOMove(endPos, moveSpeed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _isMoving = false;
                        currentTile = nextTile;

                        _animator.SetBool("isMoving", false);
                    });
        }
    }
    public void TurnOnDiceNumber(int index)
    {
        TurnOffDiceNumber();

        if (_diceNumberObjects[index - 1] != null)
        {
            _diceNumberObjects[index - 1].SetActive(true);
        }

    }
    public void TurnOffDiceNumber()
    {
        foreach (var diceNumberObject in _diceNumberObjects)
        {
            diceNumberObject.SetActive(false);
        }

    }
    public void TurnOnDice()
    {
        _dice.gameObject.SetActive(true);
    }
    public void TurnOffDice()
    {
        _dice.gameObject.SetActive(false);
    }

    private void OnDiceInputReceived(int receivedID)
    {
        if (receivedID != playerID)
        {
            return;
        }
        BoardManager.Instance.OnPlayersInput(this);
    }    
}
