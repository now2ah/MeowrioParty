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

    [SerializeField] private NetworkVariable<GameState> _currentState;
    [SerializeField] private int _maxRound;
    [SerializeField] private int _currentRound;

    [SerializeField] private NetworkList<ulong> _turnOrder; //순서 정해서 여기에 넣기 

    [SerializeField] private Board _board;
    [SerializeField] private ulong _currentPlayerId;
    [SerializeField] private bool _canInput = false;

    private int _currentPlayerIndex; // 현재 움직이고 있는 플레이어의 인덱스
    private Dictionary<ulong, int> _playerDiceNumberList = new(); //순서뽑을 때 필요한 tempDic

    [SerializeField] private List<GameObject> _spawnPointList;

    //temporary
    public List<GameObject> characterPrefabList;

    public override void Awake()
    {
        base.Awake();

        _maxRound = 2;
        _currentRound = 0;
        _currentPlayerIndex = 0;
        _turnOrder = new NetworkList<ulong>();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

        if (IsServer)
        {
            _currentState.Value = GameState.GameReady;
        }

        CameraManager.Instance.ChangeCamera(0);

        StartOpeningSequence();
    }

    private void StartOpeningSequence()
    {
        StartCoroutine(OpeningCo());
    }

    private IEnumerator OpeningCo()
    {
        CameraManager.Instance.ChangeCamera(1);
        yield return new WaitForSeconds(2);

        _canInput = true;
        //주사위를 On 시킨다
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        if (!IsServer)
            return;

        int prefabIndex = NetworkManager.Singleton.ConnectedClientsList.Count - 1;
        Vector3 spawnPos = _spawnPointList[prefabIndex].transform.position;
        GameObject playerObj = Instantiate(characterPrefabList[prefabIndex], spawnPos, Quaternion.identity);
        playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        Player player = playerObj.GetComponent<Player>();
        player.currentTile = _board.tiles[0];
        _playerDiceNumberList[clientId] = -1;
    }

    [Rpc(SendTo.Server)] //클 -> 서 주사위 굴려줘
    public void RequestRollDiceServerRpc(ulong clientId)
    {
        if (!_canInput)
            return;

        if (_currentState.Value == GameState.GameReady)
        {
            int diceValue = UnityEngine.Random.Range(1, 7);
            _playerDiceNumberList[clientId] = diceValue;

            Debug.Log("cliendId : " + clientId + ", diceValue : " + diceValue);

            //주사위 숫자 연출 (주사위 off / 숫자 Sprite 출력)
            RollDiceSequenceRpc(clientId, diceValue);

            if (_playerDiceNumberList.All(p => p.Value != -1))
            {
                SetTurnOrder();
                _currentState.Value = GameState.GamePlay;
                _currentPlayerId = _turnOrder[0];

                //Player들 시작 타일로 이동
                foreach (var connectedClient in NetworkManager.Singleton.ConnectedClients)
                {
                    connectedClient.Value.PlayerObject.transform.position = _board.tiles[0].transform.position;
                }

                //첫번째 턴 Player의 정면 카메라 On
                //플레이어의 턴 시작 시점에 주사위 켜기
                TogglePlayerDiceRpc(_currentPlayerId, true);
            }
        }
        else if (_currentState.Value == GameState.GamePlay)
        {
            if (!IsPlayersTurn(clientId)) return;
            //굴리고 이동 

            int diceValue = UnityEngine.Random.Range(1, 7);

            TogglePlayerDiceRpc(clientId, false);

            StartCoroutine(SendTileCo(_turnOrder[_currentPlayerIndex], diceValue));
        }
    }

    [Rpc(SendTo.Server)]
    private void RollDiceSequenceRpc(ulong clientId, int diceValue)
    {
        TogglePlayerDiceRpc(clientId, false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TogglePlayerDiceRpc(ulong clientId, bool isOn)
    {
        var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();
        player._dice.gameObject.SetActive(isOn);
    }

    private void SetTurnOrder()
    {
        var sorted = _playerDiceNumberList.OrderByDescending(p => p.Value).ToList();
        _turnOrder.Clear();
        foreach (var tempDic in sorted)
        {
            _turnOrder.Add(tempDic.Key);
        }
    }
    private bool IsPlayersTurn(ulong clientId)
    {
        return clientId == _currentPlayerId;
    }

    private IEnumerator SendTileCo(ulong playerId, int diceValue)
    {
        var player = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject.GetComponent<Player>();
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

        if (_currentPlayerIndex >= _turnOrder.Count)
        {
            _currentPlayerIndex = 0;
            _currentRound++;
            if (_currentRound >= _maxRound)
            {
                _currentState.Value = GameState.GameEnd;
                return;
            }
        }

        _currentPlayerId = _turnOrder[_currentPlayerIndex];
        TogglePlayerDiceRpc(_currentPlayerId, true);
    }
}
