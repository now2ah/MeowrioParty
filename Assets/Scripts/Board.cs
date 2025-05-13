using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    List<Tile> boardTiles;

    private void Awake()
    {
        boardTiles = new List<Tile>();
    }
}
