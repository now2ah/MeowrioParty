using System.Collections;
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

    private Dice _dice;

    private void Start()
    {
        
    }

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
        for (int i = 0; i < diceValue; i++)
        {
            nextIndex = (++index) % BoardManager.Instance.board.tiles.Length;
            Transform destination = BoardManager.Instance.board.tiles[nextIndex].transform;

            transform.position = destination.position;

            yield return new WaitForSeconds(1f);
        }
        currentTile = BoardManager.Instance.board.tiles[nextIndex];
    }

    public void DiceValueIndicator(int diceValue)
    {

    }
}
