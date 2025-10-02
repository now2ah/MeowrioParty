using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;
using Meowrio.Manager;
using Meowrio.Domain;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private DiceController _diceController;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>();
    [SerializeField] private GameObject _coinPlusUI;
    [SerializeField] private GameObject _coinMinusUI;
    private Animator _animator;

    public bool IsMoving { get; private set; }
    [SerializeField] private float moveSpeed = 3f;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        
    }

    private void OnEnable()
    {
        BoardGameManager.Instance.OnSetTurnOrderStateStarted += BoardGameManager_OnSetTurnOrderStateStarted;
        BoardGameManager.Instance.OnRollDiceForSetTurnOrder += BoardGameManager_OnRollDiceForSetTurnOrder;
    }

    private void OnDisable()
    {
        BoardGameManager.Instance.OnSetTurnOrderStateStarted -= BoardGameManager_OnSetTurnOrderStateStarted;
        BoardGameManager.Instance.OnRollDiceForSetTurnOrder -= BoardGameManager_OnRollDiceForSetTurnOrder;
    }

    private void BoardGameManager_OnSetTurnOrderStateStarted()
    {
        ToggleDiceRpc(true);
    }

    private void BoardGameManager_OnRollDiceForSetTurnOrder(int playerID, int diceNumber)
    {
        if (playerID == (int)NetworkObjectId)
            RollDiceSequenceRpc(diceNumber);
    }

    void Start()
    {
        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(200, 50);
    }

    [Rpc(SendTo.Everyone)]
    public void MoveToRpc(Vector3 position, Quaternion rotation)
    {
        gameObject.transform.position = transform.position;
        gameObject.transform.rotation = transform.rotation;
    }

    [Rpc(SendTo.Everyone)]
    public void MoveToRpc(int nextTileIndex)
    {
        //gameObject.transform.position = nextTile.transform.position;
        //gameObject.transform.position = nextTile.transform.position;
    }

    public void MoveTo(TileController nextTile)
    {
        if (!IsMoving)
        {
            IsMoving = true;
            _animator.SetBool("isMoving", true);
            Vector3 endPos = nextTile.transform.position;
            transform.DOLookAt(endPos, 0.2f);

            transform.DOMove(endPos, moveSpeed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        IsMoving = false;
                        _animator.SetBool("isMoving", false);
                    });
        }
    }

    [Rpc(SendTo.Everyone)]
    public void RollDiceSequenceRpc(int diceValue)
    {
        StartCoroutine(RollDiceSequenceCoroutine(diceValue));
    }

    IEnumerator RollDiceSequenceCoroutine(int diceValue)
    {
        ToggleDiceRpc(false);
        TurnOnDiceNumberRpc(diceValue);
        yield return null;
    }

    [Rpc(SendTo.Everyone)]
    public void ToggleDiceRpc(bool isOn)
    {
        _diceController.gameObject.SetActive(isOn);
        if (isOn)
        {
            _diceController.PlayAnimation(AnimationType.Roll);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void TurnOnDiceNumberRpc(int index)
    {
        if (_diceNumberObjects[index - 1] != null)
        {
            _diceNumberObjects[index - 1].SetActive(true);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void TurnOffDiceNumberRpc()
    {
        foreach (var diceNumberObject in _diceNumberObjects)
        {
            diceNumberObject.SetActive(false);
        }
    }
    [Rpc(SendTo.Everyone)]
    public void TurnOnCoinPlusRpc()
    {
        StartCoroutine(TurnOnCoinSecCo(_coinPlusUI));
    }

    [Rpc(SendTo.Everyone)]
    public void TurnOnCoinMinusRpc()
    {
        StartCoroutine(TurnOnCoinSecCo(_coinMinusUI));
    }

    private IEnumerator TurnOnCoinSecCo(GameObject go)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(3f);
        go.SetActive(false);
    }

    public void TransportPlayer(TileController tile)
    {
        gameObject.transform.position = tile.gameObject.transform.position;
    }

    [Rpc(SendTo.Everyone)]
    public void PlayAnimationRpc(string trigger)
    {
        _animator.SetTrigger(trigger);
    }
}
