using System.Collections.Generic;
using UnityEngine;

public class Board :MonoBehaviour
{
    //public Tile[] Tiles { get; private set; }
    //[SerializeField]
    public Tile[] tiles;

    [SerializeField]
    private VoidEventChannelSO OnGameStart;

    private void OnEnable()
    {
        OnGameStart.OnEventRaised += InitializeBoard;
    }
    private void OnDisable()
    {
        OnGameStart.OnEventRaised -= InitializeBoard;
    }


    private void InitializeBoard()
    {

    }
}
