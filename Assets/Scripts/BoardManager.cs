using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class BoardManager : NetSingleton<BoardManager>
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

    //temporary
    public List<GameObject> characterList;

    private int _localPlayerNumber;

    public override void Awake()
    {
        base.Awake();

        _maxRound = 2;
        _currentRound = 0;
        _currentPlayerIndex = 0;
        _currentPlayer = null;

        //player 순서 정하는 list 초기화
        for (int i = 0; i < _playerList.Count; ++i)
        {
            (int, Player) playerDiceNumber;

            playerDiceNumber.Item1 = -1;
            playerDiceNumber.Item2 = _playerList[i];
            _playerDiceNumberList.Add(playerDiceNumber);
        }

        _phaseMachine = gameObject.AddComponent<PhaseMachine>();
        _readyPhase = new GameReadyPhase(this, EGamePhase.GameReady);
        _playPhase = new GamePlayPhase(this, EGamePhase.GamePlay);
        _endPhase = new GameEndPhase(this, EGamePhase.GameEnd);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnConnectionEvent += (networkManager, connectionEventData) =>
        {
            Debug.Log($"Connected : {connectionEventData.ClientId} {connectionEventData.EventType}");
        };
    }

    public override void OnNetworkSpawn()
    {
        ulong playerNumber = NetworkManager.Singleton.LocalClientId;

        //if client connected
        if (NetworkManager.Singleton.IsHost)
        {
            //player 0
            _localPlayerNumber = 0;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            //player 1
            _localPlayerNumber = 1;
        }

        if (IsServer)
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;

        StartBoardGame();
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        //if all clients are connected
        //need to be changed later (hard coding)
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            CreatePlayersRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void CreatePlayersRpc()
    {
        GameObject player0Obj = Instantiate(characterList[0], Vector3.zero, Quaternion.identity);
        Player player0 = player0Obj.GetComponent<Player>();
        _playerList[0] = player0;

        GameObject player1Obj = Instantiate(characterList[1], Vector3.zero, Quaternion.identity);
        Player player1 = player1Obj.GetComponent<Player>();
        _playerList[1] = player1;
    }

    private void StartBoardGame()
    {
        _phaseMachine.StartPhase(_readyPhase);
    }

    private void ProcessDiceForTurnOrderButton(int playerIndex)
    {
        if (_playerDiceNumberList[playerIndex].Item1 == -1)
        {
            int playersDiceNum = _playerList[playerIndex].RollDice();
            AddToPlayerOrderListRpc(playerIndex, playersDiceNum);
        }

        if (IsAllPlayerRolledDiceForOrder())
        {
            SetTurnOrder();
            _phaseMachine.ChangePhase(_playPhase);
        }
    }

    [Rpc(SendTo.Server)]
    private void AddToPlayerOrderListRpc(int playerIndex, int diceValue)
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

    public void OnPlayersInput(Player player/*, int receivedID*/)
    {
        //if (_localPlayerNumber == receivedID)
        {
            if (_phaseMachine.IsPhase(_readyPhase))
            {
                ProcessDiceForTurnOrderButton(player.playerID);
            }
            else if (_phaseMachine.IsPhase(_playPhase) && IsPlayersTurn(player))
            {
                ProcessConfirmButton();
            }
        }
    }

    private bool IsPlayersTurn(Player player)
    {
        return player == _currentPlayer;
    }
}
