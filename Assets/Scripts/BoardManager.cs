using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    // 순서 정하고
    // 메인게임
    // 게임 종료
    GameReady,
    GamePlaying,
    GameEnd
}
public class BoardManager : Singleton<BoardManager>
{
    [SerializeField]
    private int _maxGameTurn;
    [SerializeField]
    private int _currentGameTurn;   
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
    [SerializeField]
    private VoidEventChannelSO onSetOrderRollDice;
    [SerializeField]
    private VoidEventChannelSO onMainGameStarted;


    List<(int, Player)> playerDiceNumberList = new List<(int, Player)>();
    private int _setOrderIndex;
    private Player _currentPlayer;
    private bool _isTurnStarted;

    private GameState _currentState;

    private int _currentMovingPlayerIndex = 0;    // Index of Player who is moving now

    public override void Awake()
    {
        base.Awake();
        //players = new List<Player>();
        _dice = _diceObj.GetComponent<Dice>();
        _setOrderIndex = 0;
        _isTurnStarted = false;
        _currentState = GameState.GameReady;
        _currentGameTurn = 0;
    }

    private void OnEnable()
    {
        OnRollDice.OnEventRaised += OnRollDice_OnEventRaised;
        onSetOrderRollDice.OnEventRaised += SetPlayerTurnOrder;
        //onMainGameStarted.OnEventRaised += OnMainGameStarted_OnEventRaised;
    }

    private void OnMainGameStarted_OnEventRaised()
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        OnRollDice.OnEventRaised -= OnRollDice_OnEventRaised;
        onSetOrderRollDice.OnEventRaised -= SetPlayerTurnOrder;
    }

    private void OnRollDice_OnEventRaised(int diceValue)
    {
        if(_currentState == GameState.GameReady)
        {
            (int, Player) playerDiceNumber;
        
            playerDiceNumber.Item1 = diceValue;
            playerDiceNumber.Item2 = _playerList[_setOrderIndex];

            playerDiceNumberList.Add(playerDiceNumber);
            _setOrderIndex++;
            if(_setOrderIndex == _playerList.Count)
            {
                onSetOrderRollDice.RaiseEvent();
            }

        }
        else if (_currentState == GameState.GamePlaying)
        {
            //int playerListIndex = _currentMovingPlayerIndex;
            //int playerListIndex = _currentMovingPlayerIndex % _playerList.Count;
            _playerList[_currentMovingPlayerIndex].Move(diceValue);
            Debug.Log("current turn is " + _currentGameTurn);
            _currentMovingPlayerIndex++;
            if(_currentMovingPlayerIndex == _playerList.Count)
            {
                _currentMovingPlayerIndex = 0;
                _currentGameTurn++;
            }
            if(_currentGameTurn == _maxGameTurn)
            {
                _currentState = GameState.GameEnd;
            }
        }
        else
        {

        }
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
        _currentGameTurn = 0;
        _maxGameTurn = 2;


    }

    private void StartGame()
    {
        OnGameStart.RaiseEvent();
        //PlayGame();
    }

    void SetPlayerTurnOrder()
    {
        playerDiceNumberList.Sort((a, b) => b.Item1.CompareTo(a.Item1));

        for (int i = 0; i < playerDiceNumberList.Count; i++)
        {
            _playerList[i] = playerDiceNumberList[i].Item2;
        }
        onMainGameStarted.RaiseEvent();
        _currentState = GameState.GamePlaying;
    }
    
    private void PlayGame()
    {
        
    }

    private IEnumerator GameLoopCoroutine()
    {
        for (int i = 0; i < _maxGameTurn; i++)
        {
            for (int j = 0; j < _playerList.Count; j++)
            {                
                yield return new WaitForSeconds(10f);

                //int diceValue = _dice.RollDice();
                //_playerList[j].Move(diceValue);
            }
        }

    }

    private void EndGame()
    {

    }
}
