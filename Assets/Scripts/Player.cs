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
    public void MoveToNextTile(Tile nextTile)
    {
        StartCoroutine(MoveCoroutine(nextTile));
    }

    public int RollDice()
    {
        return _dice.Roll();
    }

    // comment: 아래 로직은 필히 없어져야 함.
    private IEnumerator MoveCoroutine(Tile nextTile)
    {
        transform.position = nextTile.transform.position;
        currentTile = nextTile;
        yield return new WaitForSeconds(1f);
    }
}
