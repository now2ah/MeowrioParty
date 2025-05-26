using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
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
    public InputManagerSO inputManager;

    [SerializeField] private NetworkVariable<GameState> _currentState;
    [SerializeField] private int _maxRound;
    [SerializeField] private NetworkVariable<int> _currentRound;
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
    public Dictionary<ulong, PlayerController> _playerCtrlMap = new();

    public override void Awake()
    {
        base.Awake();

        _maxRound = 2;
        _turnOrder = new NetworkList<ulong>();

        InitManager();

        if (NetworkManager.Singleton.IsServer)
        {
            var networkObject = GetComponent<NetworkObject>();
            networkObject.Spawn(true);      // NetworkObject가 부착된 BoardManager가 부착된 게임오브젝트 스폰
        }
    }

    private void InitManager()
    {
        // HACK: 싱글톤 인스턴스 초기화 순서를 고정하기 위하여 
        CameraManager.Instance.gameObject.SetActive(true);
        SoundManager.Instance.gameObject.SetActive(true);
    }

    public override void OnNetworkSpawn()
    {
        //need to remove
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;

        // 네트워크상에 스폰 시 초기값 설정 및 InitializePlayer에 개별 ClientID 전달
        if (IsServer)
        {
            _currentPlayerIndex = 0;
            _currentState.Value = GameState.GameReady;
            _currentRound.Value = 0;

        }
        CameraManager.Instance.ChangeCamera(0);
        inputManager.OnConfirmButtonPerformed += GetInput;

        if (IsServer)
        {
            StartOpeningSequenceRpc();
        }
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            LeaderBoardManager.Instance.InitializeLeaderBoard(2);
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                InitalizePlayerRpc(clientId); // 여기에 개별 ClientId 전달
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void StartOpeningSequenceRpc()
    {
        UIManager.Instance.OpenNoticeUISec("순서를 정해보죠!", 3f);
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

    // 씬 로드 시점에서 ClientID를 받아 플레이어 생성 
    private void InitalizePlayerRpc(ulong clientId)
    {
        Debug.Log("InitializePlayerRpc : " + clientId);
        if (!IsServer)
            return;

        int prefabIndex = (int)clientId;    //need to fix
        Vector3 spawnPos = _spawnPointList[prefabIndex].transform.position;

        GameObject playerObj = Instantiate(characterPrefabList[prefabIndex], spawnPos, Quaternion.identity);
        playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        PlayerController playerCtrl = playerObj.GetComponent<PlayerController>();

        _playerCtrlMap[clientId] = playerCtrl;
        _playerDataMap[clientId] = new PlayerData(clientId);
        _playerDataMap[clientId].currentTile = _board.tileControllers[0];

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
        _currentPlayerIndex++;

        if (_currentPlayerIndex == _turnOrder.Count)
        {
            _currentPlayerIndex = 0;
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
        _currentPlayerId = _turnOrder[_currentPlayerIndex];
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
