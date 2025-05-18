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
    public InputManagerSO inputManager;

    [SerializeField] private int _maxRound; //테스트를 위해 인스펙터 노출 추후 수정 예정
    [SerializeField] private int _currentRound;
    [SerializeField] private List<Player> _playerList;
    [SerializeField] private Board _board;
    [SerializeField] private Player _currentPlayer;

    private int _currentPlayerIndex; // 현재 움직이고 있는 플레이어의 인덱스
    private List<(int, Player)> _playerDiceNumberList = new List<(int, Player)>();
    private PhaseMachine _phaseMachine;
    private GameReadyPhase _readyPhase;
    private GamePlayPhase _playPhase;
    private GameEndPhase _endPhase;

    public Board Board { get { return _board; } }   //_board를 클래스 외부에서 쓸 수 있도록 하는 Property

    public override void Awake()
    {
        base.Awake();

        _maxRound = 2;
        _currentRound = 0;
        _currentPlayerIndex = 0;
        //players = new List<Player>();
        //initialize board
        _currentPlayer = null;

        //player 순서 정하는 list 초기화
        for (int i = 0; i < _playerList.Count; ++i)
        {
            (int, Player) playerDiceNumber;

            playerDiceNumber.Item1 = -1;
            playerDiceNumber.Item2 = _playerList[i];
            _playerDiceNumberList.Add(playerDiceNumber);
        }

        _phaseMachine = new PhaseMachine();
        _readyPhase = new GameReadyPhase(this);
        _playPhase = new GamePlayPhase(this);
        _endPhase = new GameEndPhase(this);

        inputManager.OnConfirmButtonPerformed += InputManager_OnConfirmButtonPerformed;

        //temporary code before applying network feature
        inputManager.OnPlayer0DiceButtonPerformed += InputManager_OnPlayer0DiceButtonPerformed;
        inputManager.OnPlayer1DiceButtonPerformed += InputManager_OnPlayer1DiceButtonPerformed;
    }

    private void Start()
    {
        StartBoardGame();
    }

    private void Update()
    {
        if (_phaseMachine != null && _phaseMachine.IsRunning)
        {
            _phaseMachine.UpdatePhase();
        }
    }

    private void StartBoardGame()
    {
        _phaseMachine.StartPhase(_readyPhase);
    }

    //temporary code before applying network feature
    private void InputManager_OnPlayer0DiceButtonPerformed(object sender, bool e)
    {
        if (_phaseMachine.IsPhase(_readyPhase))
        {
            ProcessDiceForTurnOrderButton(0);
        }
    }

    //temporary code before applying network feature
    private void InputManager_OnPlayer1DiceButtonPerformed(object sender, bool e)
    {
        if (_phaseMachine.IsPhase(_readyPhase))
        {
            ProcessDiceForTurnOrderButton(1);
        }
    }

    //temporary code before applying network feature
    private void ProcessDiceForTurnOrderButton(int playerIndex)
    {
        if (_playerDiceNumberList[playerIndex].Item1 == -1)
        {
            int playersDiceNum = _playerList[playerIndex].RollDice();
            AddToPlayerOrderList(playerIndex, playersDiceNum);
        }

        if (IsAllPlayerRolledDiceForOrder())
        {
            SetTurnOrder();
            _phaseMachine.ChangePhase(_playPhase);
        }
    }

    private void AddToPlayerOrderList(int playerIndex, int diceValue)
    {
        (int, Player) playerDiceNumber;

        playerDiceNumber.Item1 = diceValue;
        playerDiceNumber.Item2 = _playerList[playerIndex];

        _playerDiceNumberList[playerIndex] = playerDiceNumber;
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

    private void SetTurnOrder()
    {
        _playerDiceNumberList.Sort((a, b) => b.Item1.CompareTo(a.Item1));

        for (int i = 0; i < _playerDiceNumberList.Count; i++)
        {
            _playerList[i] = _playerDiceNumberList[i].Item2;
        }

        _phaseMachine.ChangePhase(_playPhase);
        _currentPlayer = _playerList[0];

        TurnOnDiceOnCurrentPlayer();    // 플레이어의 턴 시작 시점에 주사위 켜기
    }


    private void InputManager_OnConfirmButtonPerformed(object sender, bool e)
    {
        if (_phaseMachine.IsPhase(_playPhase))
        {
            ProcessConfirmButton();
        }
    }

    //private void ProcessConfirmButton()
    //{
    //    _currentPlayer = _playerList[_currentPlayerIndex];
    //    //_currentPlayer._dice.gameObject.SetActive(false);
    //    ProcessTurn(_currentPlayer);
    //    _currentPlayerIndex++;

    //    if (_currentPlayerIndex == _playerList.Count)
    //    {
    //        _currentPlayerIndex = 0;
    //        _currentRound++;
    //    }

    //    if (_currentRound == _maxRound)
    //    {
    //        _phaseMachine.ChangePhase(_endPhase);
    //    }
    //}

    //private void ProcessTurn(Player currentPlayer)
    //{
    //    int playersDiceNum = _currentPlayer.RollDice(); //주사위 던지기
    //    StartCoroutine(SendTileCo(currentPlayer, playersDiceNum)); //움직여
    //}
    private void ProcessConfirmButton()
    {
        _currentPlayer = _playerList[_currentPlayerIndex];

        _currentPlayer.TurnOffDice(); // 주사위 끄기
        int playersDiceNum = _currentPlayer.RollDice(); // 주사위 굴리기
        StartCoroutine(SendTileCo(_currentPlayer, playersDiceNum)); // 이동 시작

        _currentPlayerIndex++;

        if (_currentPlayerIndex == _playerList.Count)
        {
            _currentPlayerIndex = 0;
            _currentRound++;
        }

        if (_currentRound == _maxRound)
        {
            _phaseMachine.ChangePhase(_endPhase);
        }
    }


    private IEnumerator SendTileCo(Player player, int diceValue)
    {
        int tileIndex = player.currentTile.tileIndex;

        for (int i = 0; i < diceValue; i++) //한 타일씩 -> 나중에 갈림길 고려
        {
            int nextIndex = (tileIndex + 1) % _board.tiles.Length;
            Tile nextTile = _board.tiles[nextIndex];

            player.MoveTo(nextTile);
            player.TurnOnDiceNumber(diceValue - i);
            tileIndex = nextIndex;

            while (player.IsMoving)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            player.TurnOffDiceNumber();
        }

        // 이동이 끝난 뒤 다음 플레이어 주사위 자동 켜기
        if (_currentRound < _maxRound)
        {
            _currentPlayer = _playerList[_currentPlayerIndex]; // 다음 플레이어 갱신
            TurnOnDiceOnCurrentPlayer();
        }
    }

    private void TurnOnDiceOnCurrentPlayer()
    {
        // 모든 플레이어의 주사위 끄고, 현재 플레이어 것만 킴
        foreach (Player player in _playerList)
        {
            player.TurnOffDice();
        }
        _currentPlayer.TurnOnDice();
    }
}
