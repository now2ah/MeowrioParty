using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
// COMMENT : 플레이어 이동과 관련된 로직만 처리하면 됨
// COMMENT : BoardManager가 가야할 Tile을 알려줌

public class Player : MonoBehaviour
{
    public Tile currentTile; // COMMENT: 게임 진행과 관련된 부분이므로 BoardManager가 갖고 있어야 함.

    [SerializeField] public Dice _dice;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>(); // Comment: 아래 변수도 불필요

    private bool _isMoving = false;
    [SerializeField] private float moveSpeed = 3f;

    public bool IsMoving { get { return _isMoving; } }

    void Start()
    {
        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(200, 50);
    }
    public int RollDice()
    {
        _dice.gameObject.SetActive(true);
        return _dice.Roll();
    }

    public void MoveTo(Tile nextTile)
    {
        if (!_isMoving)
        {
            _isMoving = true;
            Vector3 endPos = nextTile.transform.position;
            transform.DOMove(endPos, moveSpeed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _isMoving = false;
                        currentTile = nextTile;
                    });
        }
    } 
}
