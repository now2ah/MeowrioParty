using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private int _playerID;
    private string _playerName;

    public Tile currentTile;
    //public Tile nextTile;
    //public Board board;

    public int order;

    public Dice _dice;

    [SerializeField]
    private List<GameObject> _diceNumberObjects = new List<GameObject>();


    public void Initialize(Tile startTile)
    {
        //currentTile = startTile;
        //transform.position = currentTile.transform.position;
    }


    public void Move(int diceValue) 
    {
        //int nextTileIndex = currentTile.tileIndex + diceValue;

        StartCoroutine(MoveCoroutine(diceValue));
    }

    private IEnumerator MoveCoroutine(int diceValue)
    {
        int index = currentTile.tileIndex;
        int nextIndex = 0;
        int leftIndex = diceValue-1;
        for (int i = 0; i < diceValue; i++)
        {
            _diceNumberObjects[leftIndex].SetActive(true);

            nextIndex = (++index) % BoardManager.Instance.board.tiles.Length;
            Transform destination = BoardManager.Instance.board.tiles[nextIndex].transform;
            transform.position = destination.position;
            yield return new WaitForSeconds(1f);
            _diceNumberObjects[leftIndex].SetActive(false);
            leftIndex--;
        }
        currentTile = BoardManager.Instance.board.tiles[nextIndex];
    }
}
