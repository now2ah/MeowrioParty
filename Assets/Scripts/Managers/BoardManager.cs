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
    private NetworkVariable<bool> _canInput = new NetworkVariable<bool>(false);

    private int _currentPlayerIndex; // 현재 움직이고 있는 플레이어의 인덱스
    private Dictionary<ulong, int> _playerDiceNumberList = new(); //순서뽑을 때 필요한 tempDic

    [SerializeField] private List<GameObject> _spawnPointList;

    //temporary
    public List<GameObject> characterPrefabList;

    private Dictionary<ulong, PlayerData> _playerDataMap = new();
    private Dictionary<ulong, PlayerController> _playerCtrlMap = new();
    private Dictionary<int, TileController> _tileCtrlMap = new();
    private Dictionary<int, Tile> _tileDataMap = new();


    public override void Awake()
    {
        base.Awake();

        _maxRound = 2;
        _currentRound = 0;
        _currentPlayerIndex = 0;
        _turnOrder = new NetworkList<ulong>();

        //Test용 초기화
        for (int i = 0; i < _board.tileControllers.Length; i++)
        {
            switch (_board.tileControllers[i].tileType)
            {
                case ETileType.CoinTile:
                    _tileDataMap[i] = new CoinTile(i);;
                    _tileCtrlMap[i] = _board.tileControllers[i];
                    break;
                case ETileType.StarTile:
                    _tileDataMap[i] = new StarTile(i);
                    _tileCtrlMap[i] = _board.tileControllers[i];
                    break;
                case ETileType.WarpTile:
                    _tileDataMap[i] = new WarpTile(i);
                    _tileCtrlMap[i] = _board.tileControllers[i];
                    break;
                default:
                    _tileDataMap[i] = new CoinTile(i);
                    _tileCtrlMap[i] = _board.tileControllers[i];
                    break;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

        if (IsServer)
        {
            _currentState.Value = GameState.GameReady;

        }
        CameraManager.Instance.ChangeCamera(0);
        inputManager.OnConfirmButtonPerformed += GetInput;
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

        if (IsServer)
            _canInput.Value = true;
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

        PlayerController playerCtrl = playerObj.GetComponent<PlayerController>();

        _playerCtrlMap[clientId] = playerCtrl;
        _playerDataMap[clientId] = new PlayerData(clientId);
        _playerDataMap[clientId].currentTile = _tileDataMap[0];

        _playerDiceNumberList[clientId] = -1;
    }

    [Rpc(SendTo.Server)] //클 -> 서 주사위 굴려줘
    public void RequestRollDiceServerRpc(ulong clientId)
    {
        if (!_canInput.Value)
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
                    _playerCtrlMap[connectedClient.Key].TransportPlayer(_tileCtrlMap[0]);
                }

                //첫번째 턴 Player의 정면 카메라 On
                //플레이어의 턴 시작 시점에 주사위 켜기
                _playerCtrlMap[_currentPlayerId].ToggleDiceRpc(true);
            }
        }
        else if (_currentState.Value == GameState.GamePlay)
        {
            if (!IsPlayersTurn(clientId) || !_canInput.Value) return;
            //굴리고 이동 
            _canInput.Value = false;
            int diceValue = UnityEngine.Random.Range(1, 7);
            TogglePlayerDice(clientId, false);

            StartCoroutine(SendTileCo(clientId, diceValue));
        }
    }

    [Rpc(SendTo.Server)]
    private void RollDiceSequenceRpc(ulong clientId, int diceValue)
    {
        TogglePlayerDice(clientId, false);
    }


    private void TogglePlayerDice(ulong clientId, bool isOn)
    {
        if (_playerCtrlMap.TryGetValue(clientId, out var ctrl))
        {
            ctrl.ToggleDiceRpc(isOn);
        }
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
        PlayerData data = _playerDataMap[playerId];
        PlayerController controller = _playerCtrlMap[playerId];

        int tileIndex = data.currentTile.tileIndex;

        for (int i = 0; i < diceValue; i++) //한 타일씩 -> 나중에 갈림길 고려
        {
            int nextIndex = (tileIndex + 1) % _tileDataMap.Count;
            TileController nextTileObj = _tileCtrlMap[nextIndex];
            Tile nextTile = _tileDataMap[nextIndex];

            data.MoveTo(nextTile);
            controller.MoveTo(nextTileObj);
            controller.TurnOnDiceNumberRpc(diceValue - i);
            tileIndex = nextIndex;

            while (controller.IsMoving)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            controller.TurnOffDiceNumberRpc();
        }
        //tile effect 발현 - 수정 필요 
        if (_tileDataMap[tileIndex] is WarpTile)
        {
            controller.TransportPlayer(_tileCtrlMap[2]); //임의로 3번째 타일로 이동시킴
        }
        else
        {
            _tileDataMap[tileIndex].TileEvent(data, 2);
        }
        NextTurn();
        _canInput.Value = true;
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
        TogglePlayerDice(_currentPlayerId, true);
    }

    //Input도 나중에 아예 분리해내면 좋을 것 같습니다.
    private void GetInput(object sender, bool isPressed)
    {
        if (!isPressed) return;

        RequestRollDiceServerRpc(NetworkManager.Singleton.LocalClientId);
        Debug.Log(NetworkManager.Singleton.LocalClientId);
    }

}
