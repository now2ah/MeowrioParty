using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private int maxTurn;
    private int currentTurn;

    List<Player> players;

    private void Awake()
    {
        players = new List<Player>();
    }

    private void InitialGameSetting()
    {
        currentTurn = 0;
        maxTurn = 10;
    }

    private void Start()
    {
        InitialGameSetting();
        StartGame();
    }

    private void StartGame()
    {
        while (true)
        {
            if (currentTurn == maxTurn)
            {
                break;
            }
            foreach (Player player in players)
            {
                //player.move();
            }
            currentTurn++;
        }
    }
    private void EndGame()
    {

    }
}
