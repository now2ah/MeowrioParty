using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GamePhase
{
    GameReady,
    GamePlay,
    GameEnd
}
public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private GamePhase _currentPhase;
    [SerializeField] private int _maxGameTurn;
    [SerializeField] private int _currentGameTurn;
    [SerializeField] private List<Player> _playerList;
    [SerializeField] private Board _board;
    [SerializeField] private Player _currentPlayer;

    private int _currentPlayerIndex; // Index of Player who is moving now
    private int _setTurnOrderIndex;
    private List<(int, Player)> playerDiceNumberList = new List<(int, Player)>();

    public Board Board { get { return _board; } }   //_board를 클래스 외부에서 쓸 수 있도록 하는 Property

    public override void Awake()
    {
        base.Awake();
        
        _currentPhase = GamePhase.GameReady;
        _maxGameTurn = 2;
        _currentGameTurn = 0;
        _currentPlayerIndex = 0;
        //players = new List<Player>();
        //initialize board
        _currentPlayer = null;
        _setTurnOrderIndex = 0;
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (_currentPhase == GamePhase.GameReady)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_setTurnOrderIndex == _playerList.Count)
                {
                    SetPlayerTurnOrder();
                    SetGamePhase(GamePhase.GamePlay);
                }
                else
                {
                    int playersDiceNum = _playerList[_setTurnOrderIndex].RollDiceForOrder();
                    AddToPlayerOrderList(playersDiceNum);
                }
            }
        }
        
        if (_currentPhase == GamePhase.GamePlay)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _currentPlayer = _playerList[_currentPlayerIndex];

                _playerList[_currentPlayerIndex].RollDiceForMove();

                //int playersDiceNum = _playerList[_currentMovingPlayerIndex].RollDice();
                //_playerList[_currentMovingPlayerIndex].Move(playersDiceNum);
                _currentPlayerIndex++;

                if (_currentPlayerIndex == _playerList.Count)
                {
                    _currentPlayerIndex = 0;
                    _currentGameTurn++;
                }

                if (_currentGameTurn == _maxGameTurn)
                {
                    _currentPhase = GamePhase.GameEnd;
                }
            }
        }
    }

    private void StartGame()
    {
        SetGamePhase(GamePhase.GameReady);
    }

    private void SetPlayerTurnOrder()
    {
        playerDiceNumberList.Sort((a, b) => b.Item1.CompareTo(a.Item1));

        for (int i = 0; i < playerDiceNumberList.Count; i++)
        {
            _playerList[i] = playerDiceNumberList[i].Item2;
        }

        _currentPhase = GamePhase.GamePlay;
        _currentPlayer = _playerList[0];
    }

    private void SetGamePhase(GamePhase phase)
    {
        _currentPhase = phase;
    }

    private void AddToPlayerOrderList(int diceValue)
    {
        (int, Player) playerDiceNumber;

        playerDiceNumber.Item1 = diceValue;
        playerDiceNumber.Item2 = _playerList[_setTurnOrderIndex];

        playerDiceNumberList.Add(playerDiceNumber);
        _setTurnOrderIndex++;
    }
}
