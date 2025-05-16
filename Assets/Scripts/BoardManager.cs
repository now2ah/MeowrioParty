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
    [SerializeField] private int _maxGameTurn; //테스트를 위해 인스펙터 노출 추후 수정 예정
    [SerializeField] private int _currentGameTurn;
    [SerializeField] private List<Player> _playerList;
    [SerializeField] private Board _board;
    [SerializeField] private Player _currentPlayer;

    private int _currentPlayerIndex; // 현재 움직이고 있는 플레이어의 인덱스스
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
                if (_setTurnOrderIndex < _playerList.Count)
                {
                    int playersDiceNum = _playerList[_setTurnOrderIndex].RollDice();
                    AddToPlayerOrderList(playersDiceNum);
                }
                else
                {
                    SetPlayerTurnOrder();
                    _currentPhase = GamePhase.GamePlay;
                }
            }
        }
        
        if (_currentPhase == GamePhase.GamePlay)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MovePlayerByTileQueue();
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

    private void MovePlayerByTileQueue()
    {
        _currentPlayer = _playerList[_currentPlayerIndex];
        _currentPlayer._dice.gameObject.SetActive(true);
        Queue<Tile> tileQueue = new Queue<Tile>();
        int playersDiceNum = _currentPlayer.RollDice();//주사위 굴려서 랜덤값 받아오고
        //그만큼 플레이어 이동 
        int index = _currentPlayer.currentTile.tileIndex + 1;
        for (int i = 0; i < playersDiceNum; i++)
        {
            tileQueue.Enqueue(_board.tiles[index]);//가야하는다음타일들을넣어서
            index++;
            (index) %= _board.tiles.Length;
        }
        _currentPlayer.GetMoveQueue(tileQueue);
    }
    private void StartGame()
    {
        _currentPhase = GamePhase.GameReady;
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

    private void AddToPlayerOrderList(int diceValue)
    {
        (int, Player) playerDiceNumber;

        playerDiceNumber.Item1 = diceValue;
        playerDiceNumber.Item2 = _playerList[_setTurnOrderIndex];

        playerDiceNumberList.Add(playerDiceNumber);
        _setTurnOrderIndex++;
    }
}
