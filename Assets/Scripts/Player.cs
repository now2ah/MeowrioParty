using UnityEngine;

public class Player : MonoBehaviour
{
    private int playerID;
    private string playerName;
    public Tile currentTile;
    public Tile nextTile;
    //public Board board;

    public void Initialize(Tile startTile)
    {
        //currentTile = startTile;
        //transform.position = currentTile.transform.position;
    }

    public void Move(int diceValue) // int steps? int diceValue?
    {
        
    }
}
