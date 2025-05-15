using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Tile currentTile; // COMMENT: 게임 진행과 관련된 부분이므로 BoardManager가 갖고 있어야 함.

    [SerializeField] private Dice _dice;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>(); // Comment: 아래 변수도 불필요

    // COMMENT : 플레이어 이동과 관련된 로직만 처리하면 됨
    // COMMENT : BoardManager가 가야할 Tile을 알려줌


    public int RollDiceForOrder()
    {
        return RollDice();
    }

    //주사위가 구르는 애니메이션이 끝난 후 플레이어 이동
    public void RollDiceForMove()
    {
        RollDice(() =>
        {
            Move(_dice.DiceValue);
        });
    }

    private int RollDice(Action callback = null)
    {
        _dice.gameObject.SetActive(true);
        return _dice.Roll(callback);
    }

    private void Move(int diceValue)
    {
        StartCoroutine(MoveCoroutine(diceValue));
    }

    // comment: 아래 로직은 필히 없어져야 함.
    private IEnumerator MoveCoroutine(int diceValue)
    {
        int index = currentTile.tileIndex;
        int nextIndex = 0;
        int leftIndex = diceValue-1;
        for (int i = 0; i < diceValue; i++)
        {
            _diceNumberObjects[leftIndex].SetActive(true);

            nextIndex = (++index) % BoardManager.Instance.Board.tiles.Length;
            Transform destination = BoardManager.Instance.Board.tiles[nextIndex].transform;
            transform.position = destination.position;
            yield return new WaitForSeconds(1f);
            _diceNumberObjects[leftIndex].SetActive(false);
            leftIndex--;
        }
        currentTile = BoardManager.Instance.Board.tiles[nextIndex];
    }
}
