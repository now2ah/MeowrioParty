using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    GameReady,
    GamePlay,
    GameEnd,
    MiniGame
}

public class BoardManager : NetSingleton<BoardManager>
{
    [SerializeField] private InputManagerSO inputManager;

    [Header("Variables")]
    [SerializeField] private NetworkVariable<GameState> _currentState;
    [SerializeField] private int _maxRound;
    [SerializeField] private NetworkVariable<int> _currentRound;
    [SerializeField] private NetworkList<ulong> _playerTurnOrder;
    [SerializeField] private ulong _currentPlayerId;

    private int _currentPlayerTurnIndex;
    private Dictionary<ulong, int> _playerDiceNumberList = new(); //순서 Dictionary

    private NetworkVariable<bool> _canInput = new NetworkVariable<bool>(false);

    [SerializeField] private Board _board;
    [SerializeField] private List<GameObject> _spawnPointList;
    [SerializeField] private List<GameObject> characterPrefabList;

    private Dictionary<ulong, PlayerData> _playerDataMap = new();
    public Dictionary<ulong, PlayerController> _playerCtrlMap = new();

    private Action OnInitializeDone;

    public override void Awake()
    {
        base.Awake();

        _maxRound = 2;
        _playerTurnOrder = new NetworkList<ulong>();

        OnInitializeDone += OnInitializeDone_StartOpeningSequenceRpc;
    }

    public override void OnNetworkSpawn()
    {
        // 네트워크상에 스폰 시 초기값 설정 및 InitializePlayer에 개별 ClientID 전달
        if (IsServer)
        {
            LeaderBoardManager.Instance.InitializeLeaderBoard(NetworkManager.Singleton.ConnectedClientsList.Count);
            
            _board = FindFirstObjectByType<Board>();
            _currentPlayerTurnIndex = 0;
            _currentState.Value = GameState.GameReady;
            _currentRound.Value = 0;

            StartCoroutine(CreatePlayerControllersCoroutine(NetworkManager.Singleton.ConnectedClientsList));
        }

        inputManager.OnConfirmButtonPerformed += GetInput;
    }

    IEnumerator CreatePlayerControllersCoroutine(IReadOnlyList<NetworkClient> clientList)
    {
        foreach (NetworkClient client in clientList)
        {
            ulong clientId = client.ClientId;
            Debug.Log("InitializePlayerRpc : " + clientId);

            int prefabIndex = (int)clientId;
            Vector3 spawnPos = _spawnPointList[prefabIndex].transform.position;

            GameObject playerObj = Instantiate(characterPrefabList[prefabIndex], spawnPos, Quaternion.identity);
            playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            while (client.PlayerObject == null)
            {
                yield return null;
            }

            PlayerController playerCtrl = playerObj.GetComponent<PlayerController>();

            _playerCtrlMap[clientId] = playerCtrl;
            _playerDataMap[clientId] = new PlayerData(clientId);
            _playerDataMap[clientId].currentTile = _board.tileControllers[0];

            _playerDiceNumberList[clientId] = -1;
        }

        OnInitializeDone?.Invoke();
    }

    [Rpc(SendTo.Everyone)]
    private void OnInitializeDone_StartOpeningSequenceRpc()
    {
        CameraManager.Instance.ChangeCameraRpc(CameraType.Board);
        UIManager.Instance.OpenNoticeUISec("파티 시작!", 3f);
        StartCoroutine(OpeningCo());
    }

    private IEnumerator OpeningCo()
    {
        yield return new WaitForSeconds(3f);
        CameraManager.Instance.ChangeCameraRpc(CameraType.Stage);
        UIManager.Instance.OpenNoticeUISec("순서를 정해보죠!", 3f);
        if (IsServer)
            _canInput.Value = true;
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
                _currentPlayerId = _playerTurnOrder[0];

                //Player들 시작 타일로 이동
                foreach (var connectedClient in NetworkManager.Singleton.ConnectedClients)
                {
                    _playerCtrlMap[connectedClient.Key].TransportPlayer(_board.tileControllers[0]);
                }

                //첫번째 턴 Player의 정면 카메라 On
                NoticeEveryoneSecRpc(_currentRound.Value + "라운드 시작~!", 3f);
                _playerCtrlMap[_currentPlayerId].ToggleDiceRpc(true);
            }
        }
        else if (_currentState.Value == GameState.GamePlay)
        {
            if (!IsPlayersTurn(clientId) || !_canInput.Value) return;
            //굴리고 이동 
            int diceValue = UnityEngine.Random.Range(1, 7);
            TogglePlayerDice(clientId, false);
            AnnounceEveryOneCloseRpc();
            StartCoroutine(SendTileCo(clientId, diceValue));
            _canInput.Value = false;
        }
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
        _playerTurnOrder.Clear();
        foreach (var tempDic in sorted)
        {
            _playerTurnOrder.Add(tempDic.Key);
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
            int nextIndex = (tileIndex + 1) % _board.tileControllers.Length;
            TileController nextTileObj = _board.tileControllers[nextIndex];

            data.MoveTo(nextTileObj);
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
        _board.tileControllers[tileIndex].TileEvent(data, controller);
        yield return new WaitForSeconds(2f);
        NextTurn();
    }


    private void NextTurn()
    {
        _canInput.Value = true;
        _currentPlayerTurnIndex++;

        if (_currentPlayerTurnIndex == _playerTurnOrder.Count)
        {
            _currentPlayerTurnIndex = 0;
            _currentRound.Value++;

            if (_currentRound.Value > _maxRound)
            {
                _currentState.Value = GameState.GameEnd;
                return;
            }
            StartMiniGame();
        }
        if (_currentState.Value != GameState.MiniGame)
            NoticeEveryoneRpc("주사위를 굴리세요");
        _currentPlayerId = _playerTurnOrder[_currentPlayerTurnIndex];
    }

    private void StartMiniGame()
    {
        NoticeEveryoneSecRpc("미니게임 시작!" + "\n" + "SPACE를 빠르게 눌러 먼저 도착하세요!^0^", 5f);
        _currentState.Value = GameState.MiniGame;
        NetworkManager.Singleton.SceneManager.LoadScene("TapRaceScene", LoadSceneMode.Additive);
    }

    public void StopMiniGame()
    {
        if (!IsServer) return;
        if (_currentRound.Value > _maxRound)
        {
            _currentState.Value = GameState.GameEnd;
            return;
        }
        _currentState.Value = GameState.GamePlay;
        Scene scene = SceneManager.GetSceneByName("TapRaceScene");
        NetworkManager.Singleton.SceneManager?.UnloadScene(scene);
        TogglePlayerDice(_currentPlayerId, true);
        NoticeEveryoneSecRpc(_currentRound.Value + "라운드 시작~!", 3f);
    }

    public void OnMiniGamePlayerFinished(ulong clientId)
    {
        if (!IsServer) return;
        NoticeEveryoneSecRpc("우승자는 " + OwnerClientId + " ㅊㅋㅊㅋ", 3f);
        StopMiniGame();
    }


    //Input도 나중에 아예 분리해내면 좋을 것 같습니다.
    private void GetInput(object sender, bool isPressed)
    {
        if (!isPressed) return;

        RequestRollDiceServerRpc(NetworkManager.Singleton.LocalClientId);
        Debug.Log(NetworkManager.Singleton.LocalClientId);
    }

    
    [Rpc(SendTo.Everyone)]
    private void AnnounceEveryOneCloseRpc()
    {
        UIManager.Instance.CloseCurrentFrontUI();
    }
    [Rpc(SendTo.Everyone)]
    private void NoticeEveryoneRpc(string message)
    {
        UIManager.Instance.OpenNoticeUI("주사위를 굴리세요");
    }

    [Rpc(SendTo.Server)]
    private void RollDiceSequenceRpc(ulong clientId, int diceValue)
    {
        TogglePlayerDice(clientId, false);
        _playerCtrlMap[clientId].TurnOnDiceNumberRpc(diceValue);
    }

    [Rpc(SendTo.Everyone)]
    private void NoticeEveryoneSecRpc(string message, float timer)
    {
        UIManager.Instance.OpenNoticeUISec(message, timer);
    }
}
