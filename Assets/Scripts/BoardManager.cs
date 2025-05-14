using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField]
    private int _maxTurn;
    [SerializeField]
    private int _currentTurn;
    [SerializeField]
    List<Player> _playerList;

    [SerializeField]
    private GameObject _diceObj;
    private Dice _dice;

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
        _dice = _diceObj.GetComponent<Dice>();

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
        _currentTurn = 0;
        _maxTurn = 10;


    }

    private void StartGame()
    {
        OnGameStart.RaiseEvent();

        SetPlayerTurnOrder();

        PlayGame();

    }

    private void PlayGame()
    {
        StartCoroutine(GameLoopCoroutine());
    }
    private IEnumerator GameLoopCoroutine()
    {
        for (int i = 0; i < _maxTurn; i++)
        {
            for (int j = 0; j < _playerList.Count; j++)
            {                
                yield return new WaitForSeconds(10f);

                int diceValue = _dice.RollDice();
                _playerList[j].Move(diceValue);
            }
        }

    }

    void SetPlayerTurnOrder()
    {
        List<(int, Player)> playerDiceNumberList = new List<(int, Player)>();

        for (int i = 0; i < _playerList.Count; ++i)
        {
            (int, Player) playerDiceNumber;

            int diceNumber = _dice.RollDice();

            playerDiceNumber.Item1 = diceNumber;
            playerDiceNumber.Item2 = _playerList[i];

            playerDiceNumberList.Add(playerDiceNumber);
        }

        playerDiceNumberList.Sort((a, b) => b.Item1.CompareTo(a.Item1));

        for (int i = 0; i < playerDiceNumberList.Count; i++)
        {
            _playerList[i] = playerDiceNumberList[i].Item2;
        }
    }

    private void EndGame()
    {

    }
}
