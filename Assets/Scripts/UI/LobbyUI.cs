using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LobbyUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startButton;

    [Header("Display")]
    [SerializeField] private TMP_Text playerListText;

    private void Start()
    {
        hostButton.onClick.AddListener(OnHostClicked);
        clientButton.onClick.AddListener(OnClientClicked);
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
        while (LobbyManager.Instance == null)
        {
            yield return null;
        }

        LobbyManager.Instance.OnPlayerListChanged += UpdatePlayerListUI;
    }

    public void OnHostClicked()
    {
        NetworkManager.Singleton.StartHost();
        readyButton.gameObject.SetActive(false);
        startButton.interactable = true;
    }

    public void OnClientClicked()
    {
        NetworkManager.Singleton.StartClient();
        readyButton.interactable = true;
        startButton.gameObject.SetActive(false);
    }

    public void OnReadyClicked()
    {
        ulong myClientId = NetworkManager.Singleton.LocalClientId;
        LobbyManager.Instance.SetReadyServerRpc(myClientId);
        readyButton.interactable = false;
    }

    public void OnStartClicked()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            ulong myClientId = NetworkManager.Singleton.LocalClientId;

            // 호스트의 Ready 상태를 먼저 설정
            LobbyManager.Instance.SetReadyServerRpc(myClientId);

            // 모든 플레이어가 준비되었는지 확인
            if (LobbyManager.Instance.AllPlayersReady())
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Board", UnityEngine.SceneManagement.LoadSceneMode.Single);
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
        foreach (var player in LobbyManager.Instance.PlayerStates)
        {
            playerListText.text += $"Player {player.ClientId} - {(player.IsReady ? "Ready" : "Not Ready")}\n";
        }
    }
}
