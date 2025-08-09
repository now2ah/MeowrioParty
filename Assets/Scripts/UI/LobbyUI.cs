using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using WebSocketSharp;

public class LobbyUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startButton;

    [Header("Display")]
    [SerializeField] private TMP_Text playerListText;

    [Header("Connection Settings")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;

    private LobbyManager _lobbyManager;

    public LobbyManager LobbyManager { get { return _lobbyManager; } set { _lobbyManager = value; } }

    private void Start()
    {
        // 버튼 기능을 코드로 등록
        startHostButton.onClick.AddListener(OnStartHostClicked);
        startClientButton.onClick.AddListener(OnStartClientClicked);
        readyButton.onClick.AddListener(OnReadyClicked);
        startButton.onClick.AddListener(OnStartClicked);

        readyButton.interactable = false;
        startButton.interactable = false;
    }
    private void OnEnable()
    {
        StartCoroutine(WaitAndRegister());
    }

    private IEnumerator WaitAndRegister()
    {
        // LobbyManager가 초기화될 때까지 대기
        while (_lobbyManager == null)
        {
            yield return null;
        }

        _lobbyManager.OnPlayerListChanged += UpdatePlayerListUI;
    }

    public void OnStartHostClicked()
    {
        _lobbyManager.ApplyConnectionSettings("127.0.0.1");
        NetworkManager.Singleton.StartHost();
        readyButton.gameObject.SetActive(true);
        startButton.interactable = true;
    }

    public void OnStartClientClicked()
    {
        _lobbyManager.ApplyConnectionSettings("127.0.0.1");
        NetworkManager.Singleton.StartClient();
        readyButton.interactable = true;
        startButton.gameObject.SetActive(false);
    }

    public void OnReadyClicked()
    {
        ulong myClientId = NetworkManager.Singleton.LocalClientId;
        _lobbyManager.SetReadyServerRpc(myClientId);
        readyButton.interactable = false;
    }

    public void OnStartClicked()
    {
        //_lobbyManager.LoadNextScene();
        
        if (NetworkManager.Singleton.IsHost)
        {
            ulong myClientId = NetworkManager.Singleton.LocalClientId;

            // 호스트의 Ready 상태를 먼저 설정
            _lobbyManager.SetReadyServerRpc(myClientId);

            // 모든 플레이어가 준비되었는지 확인
            if (_lobbyManager.IsAllPlayersReady())
            {
                NetworkManager.Singleton.SceneManager.LoadScene("BoardTest", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            else
            {
                Debug.Log("Not all players are ready yet.");
            }
        }
    }

    public void UpdatePlayerListUI()
    {
        playerListText.text = "";
        foreach (var player in _lobbyManager.playerStates)
        {
            playerListText.text += $"Player {player.ClientId} - {(player.IsReady ? "Ready" : "Not Ready")}\n";
        }
    }
}
