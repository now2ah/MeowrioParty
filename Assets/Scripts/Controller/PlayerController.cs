using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Dice _diceObj;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>();

    private Animator _animator;

    public Tile currentTile; // COMMENT: 게임 진행과 관련된 부분이므로 BoardManager가 갖고 있어야 함.
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


    public void MoveTo(Tile nextTile)
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

    
    public void ToggleDice(bool isOn)
    {
        _diceObj.gameObject.SetActive(isOn);
    }

    public void SetStartTile(Tile tile)
    {
        currentTile = tile;
        gameObject.transform.position = tile.gameObject.transform.position;
    }

}
