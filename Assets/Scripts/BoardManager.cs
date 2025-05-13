using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField]
    private int maxTurn;
    [SerializeField]
    private int currentTurn;
    [SerializeField]
    List<Player> players;

    [SerializeField]
    private GameObject diceObj;
    private Dice dice;

    //private Board board;
    public Board board;

    [SerializeField]
    private VoidEventChannelSO OnGameStart;
    [SerializeField]
    private IntEventChannelSO OnRollDice;


    public override void Awake()
    {
        base.Awake();
        //players = new List<Player>();
        dice = diceObj.GetComponent<Dice>();

    }
    private void Start()
    {
        InitializeGameSetting();
        StartGame();
    }
    private void Update()
    {

    }
    private void InitializeGameSetting()
    {
        currentTurn = 0;
        maxTurn = 10;


    }

    private void StartGame()
    {
        OnGameStart.RaiseEvent();

        SetPlayerOrder();

        PlayGame();

    }

    private void PlayGame()
    {
        StartCoroutine(DelaySec());
    }
    private IEnumerator DelaySec()
    {
        for (int i = 0; i < maxTurn; i++)
        {
            for (int j = 0; j < players.Count; j++)
            {                
                yield return new WaitForSeconds(10f);

                int diceValue = dice.RollDice();
                players[j].Move(diceValue);
            }
        }

    }

    void SetPlayerOrder()
    {
        List<(int, Player)> playerDiceNumberList = new List<(int, Player)>();

        for (int i = 0; i < players.Count; ++i)
        {
            (int, Player) playerDiceNumber;

            int diceNumber = dice.RollDice();

            playerDiceNumber.Item1 = diceNumber;
            playerDiceNumber.Item2 = players[i];

            playerDiceNumberList.Add(playerDiceNumber);
        }

        playerDiceNumberList.Sort((a, b) => b.Item1.CompareTo(a.Item1));

        for (int i = 0; i < playerDiceNumberList.Count; i++)
        {
            players[i] = playerDiceNumberList[i].Item2;
        }
    }

    private void EndGame()
    {

    }
}
