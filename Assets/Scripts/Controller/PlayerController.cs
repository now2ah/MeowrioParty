using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private DiceController _diceObj;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>();

    private Animator _animator;

    public bool IsMoving { get; private set; }
    [SerializeField] private float moveSpeed = 3f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(200, 50);
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
    [Rpc(SendTo.ClientsAndHost)]
    public void TurnOnDiceNumberRpc(int index)
    {
        //TurnOffDiceNumberRpc();

        if (_diceNumberObjects[index - 1] != null)
        {
            _diceNumberObjects[index - 1].SetActive(true);
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TurnOffDiceNumberRpc()
    {
        foreach (var diceNumberObject in _diceNumberObjects)
        {
            diceNumberObject.SetActive(false);
        }

    }

    [Rpc(SendTo.Everyone)]
    public void ToggleDiceRpc(bool isOn)
    {
        _diceObj.gameObject.SetActive(isOn);
    }

    public void TransportPlayer(TileController tile)
    {
        gameObject.transform.position = tile.gameObject.transform.position;
    }

}
