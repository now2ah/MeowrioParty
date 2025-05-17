using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private int _maxRound; //테스트를 위해 인스펙터 노출 추후 수정 예정
    [SerializeField] private int _currentRound;
    [SerializeField] private List<Player> _playerList;
    [SerializeField] private Board _board;
    [SerializeField] private Player _currentPlayer;

    private int _currentPlayerIndex; // 현재 움직이고 있는 플레이어의 인덱스
    private int _setTurnOrderIndex;
    private List<(int, Player)> _playerDiceNumberList = new List<(int, Player)>();

    public Board Board { get { return _board; } }   //_board를 클래스 외부에서 쓸 수 있도록 하는 Property

    public override void Awake()
    {
        base.Awake();

        _currentPhase = GamePhase.GameReady;
        _maxRound = 2;
        _currentRound = 0;
        _currentPlayerIndex = 0;
        //players = new List<Player>();
        //initialize board
        _currentPlayer = null;
        _setTurnOrderIndex = 0;

        //player 순서 정하는 list 초기화
        for (int i = 0; i < _playerList.Count; ++i)
        {
            (int, Player) playerDiceNumber;

            playerDiceNumber.Item1 = -1;
            playerDiceNumber.Item2 = _playerList[i];
            _playerDiceNumberList.Add(playerDiceNumber);
        }
    }

    private void Start()
    {
        StartBoardGame();
    }

    private void Update()
    {
        if (_currentPhase == GamePhase.GameReady)
        {
            //Player 0
            if (Input.GetKeyDown(KeyCode.A))
            {
                //check if rolled dice
                if (_playerDiceNumberList[0].Item1 == -1)
                {
                    int playersDiceNum = _playerList[0].RollDice();
                    AddToPlayerOrderList(0, playersDiceNum);
                }

                if (IsAllPlayerRolledDiceForOrder())
                {
                    SetTurnOrder();
                    _currentPhase = GamePhase.GamePlay;
                }
            }

            //Player 1
            if (Input.GetKeyDown(KeyCode.S))
            {
                //check if rolled dice
                if (_playerDiceNumberList[1].Item1 == -1)
                {
                    int playersDiceNum = _playerList[1].RollDice();
                    AddToPlayerOrderList(1, playersDiceNum);
                }

                if (IsAllPlayerRolledDiceForOrder())
                {
                    SetTurnOrder();
                    _currentPhase = GamePhase.GamePlay;
                }
            }
        }

        if (_currentPhase == GamePhase.GamePlay)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _currentPlayer = _playerList[_currentPlayerIndex];
                _currentPlayer._dice.gameObject.SetActive(false);
                ProcessTurn(_currentPlayer);
                _currentPlayerIndex++;

                if (_currentPlayerIndex == _playerList.Count)
                {
                    _currentPlayerIndex = 0;
                    _currentRound++;
                }

                if (_currentRound == _maxRound)
                {
                    _currentPhase = GamePhase.GameEnd;
                }
            }
        }
    }

    private void StartBoardGame()
    {
        _currentPhase = GamePhase.GameReady;
    }

    private void SetTurnOrder()
    {
        _playerDiceNumberList.Sort((a, b) => b.Item1.CompareTo(a.Item1));

        for (int i = 0; i < _playerDiceNumberList.Count; i++)
        {
            _playerList[i] = _playerDiceNumberList[i].Item2;
        }

        _currentPhase = GamePhase.GamePlay;
        _currentPlayer = _playerList[0];
    }

    private void AddToPlayerOrderList(int playerIndex, int diceValue)
    {
        (int, Player) playerDiceNumber;

        playerDiceNumber.Item1 = diceValue;
        playerDiceNumber.Item2 = _playerList[playerIndex];

        _playerDiceNumberList[playerIndex] = playerDiceNumber;
        _setTurnOrderIndex++;
    }

    private void ProcessTurn(Player currentPlayer)
    {
        int playersDiceNum = _currentPlayer.RollDice(); //주사위 던지기
        StartCoroutine(SendTileCo(currentPlayer, playersDiceNum)); //움직여
    }
    private IEnumerator SendTileCo(Player player, int diceValue)
    {
        int tileIndex = player.currentTile.tileIndex;

        for (int i = 0; i < diceValue; i++) //한 타일씩 -> 나중에 갈림길 고려
        {
            int nextIndex = (tileIndex + 1) % _board.tiles.Length;
            Tile nextTile = _board.tiles[nextIndex];

            player.MoveTo(nextTile);
            tileIndex = nextIndex;

            while (player.IsMoving)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    //if all player rolled dice
    bool IsAllPlayerRolledDiceForOrder()
    {
        for (int i = 0; i < _playerDiceNumberList.Count; ++i)
        {
            if (_playerDiceNumberList[i].Item1 == -1)
                return false;
        }

        return true;
    }
}
