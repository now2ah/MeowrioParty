using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public enum GameState
{
    GameReady,
    GamePlay,
    GameEnd,

}

public class BoardManager : NetSingleton<BoardManager>
{
    public InputManagerSO inputManager;

    private GameState _currentState;
    [SerializeField] private int _maxRound; //테스트를 위해 인스펙터 노출 추후 수정 예정
    [SerializeField] private int _currentRound;

    [SerializeField] private Dictionary<ulong, Player> _connectedClients = new Dictionary<ulong, Player>(); //연결된 클라이언트들
    [SerializeField] private NetworkList<ulong> _trunOrder; //순서 정해서 여기에 넣기 

    [SerializeField] private Board _board;
    //[SerializeField] private Player _currentPlayer;
    [SerializeField] private ulong _currentPlayerId;

    private int _currentPlayerIndex; // 현재 움직이고 있는 플레이어의 인덱스
    private Dictionary<ulong, int> _playerDiceNumberList = new(); //순서뽑을 때 필요한 tempDic

    public Board Board { get { return _board; } }   //_board를 클래스 외부에서 쓸 수 있도록 하는 Property

    //temporary
    public List<GameObject> characterList;
    private Dictionary<ulong, int> _clientPrefabMap = new();

    private int _localPlayerNumber;

    public override void Awake()
    {
        base.Awake();

        _maxRound = 2;
        _currentRound = 0;
        _currentPlayerIndex = 0;

        _trunOrder = new NetworkList<ulong>();
    }

    public override void OnNetworkSpawn()
    {
        //ulong playerNumber = NetworkManager.Singleton.LocalClientId;

        //if client connected
        if (NetworkManager.Singleton.IsHost)
        {
            //player 0
            // _localPlyerNumber = 0;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            //player 1
            _localPlayerNumber = 1;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

    }

    //[Rpc(SendTo.Server)]
    private void OnClientConnectedCallback(ulong clientId)
    {
        if (!IsServer) return;

        ulong PclientId = NetworkManager.Singleton.LocalClientId;

        int prefabIndex = _connectedClients.Count;
        GameObject playerObj = Instantiate(characterList[prefabIndex], Vector3.zero, Quaternion.identity);
        playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        _connectedClients[clientId] = playerObj.GetComponent<Player>();
        _playerDiceNumberList[clientId] = -1; //주사위 안 굴림
        Debug.Log(_connectedClients.Count);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RegisterCharacterServerRpc(int selectedIndex, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        _clientPrefabMap[clientId] = selectedIndex;
    }

    [ServerRpc] //클 -> 서 주사위 굴려줘
    public void RequestRollDiceServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (_currentState == GameState.GameReady)
        {
            int diceValue = UnityEngine.Random.Range(1, 7);
            _playerDiceNumberList[clientId] = diceValue;

            ShowDiceClientRpc(clientId, diceValue);

            if (_playerDiceNumberList.All(p => p.Value != -1))
            {
                SetTurnOrder();
                _currentState = GameState.GamePlay;
                _currentPlayerId = _trunOrder[0];

                //TurnOnDiceOnCurrentPlayer();    // 플레이어의 턴 시작 시점에 주사위 켜기
            }
        }
        if (_currentState == GameState.GamePlay)
        {
            if (!IsPlayersTurn(clientId)) return;
            //굴리고 이동 

            int diceValue = UnityEngine.Random.Range(1, 7);
            ShowDiceClientRpc(clientId, diceValue);
            StartCoroutine(SendTileCo(clientId, diceValue));
        }

    }

    [Rpc(SendTo.Everyone)]
    private void ShowDiceClientRpc(ulong clientId, int diceValue)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) //내꺼만
        {
            _connectedClients.TryGetValue(clientId, out Player player); //되나?..
            player._dice.gameObject.SetActive(true);
            player._dice.PlayDiceAnimationClient(diceValue);
        }

    }
    private void SetTurnOrder()
    {
        var sorted = _playerDiceNumberList.OrderByDescending(p => p.Value).ToList();

        _trunOrder.Clear();
        foreach (var tempDic in sorted)
        {
            _trunOrder.Add(tempDic.Key);
        }
    }

    // private void ProcessConfirmButton()
    // {
    //     _currentPlayerId = _playerList[_currentPlayerIndex];

    //     _currentPlayerId.TurnOffDice(); // 주사위 끄기
    //     int playersDiceNum = _currentPlayerId.RollDice(); // 주사위 굴리기
    //     StartCoroutine(SendTileCo(_currentPlayerId, playersDiceNum)); // 이동 시작

    //     _currentPlayerIndex++;

    //     if (_currentPlayerIndex == _playerList.Count)
    //     {
    //         _currentPlayerIndex = 0;
    //         _currentRound++;
    //     }

    //     if (_currentRound == _maxRound)
    //     {
    //         _phaseMachine.ChangePhaseRpc(_endPhase);
    //     }
    // }


    private IEnumerator SendTileCo(ulong playerId, int diceValue)
    {
        Player player = _connectedClients[playerId];

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
        NextTurn();
    }
    private void NextTurn()
    {
        _currentPlayerIndex++;
        if (_currentPlayerIndex >= _trunOrder.Count)
        {
            _currentPlayerIndex = 0;
            _currentRound++;
            if (_currentRound >= _maxRound)
            {
                _currentState = GameState.GameEnd;
                return;
            }
        }
    }

    // public void OnPlayersInput()
    // {
    //     if (_phaseMachine.IsPhase(_readyPhase))
    //     {
    //         ProcessDiceForTurnOrderButton(_localPlayerNumber);
    //     }
    //     else if (_phaseMachine.IsPhase(_playPhase) && IsPlayersTurn(_localPlayerNumber))
    //     {
    //         ProcessConfirmButton();
    //     }
    // }

    private bool IsPlayersTurn(ulong clientId)
    {
        return clientId == _currentPlayerId;
    }
}
