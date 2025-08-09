using Unity.Netcode;
using System;

public class LobbyManager : NetworkBehaviour
{
    [Serializable]
    public struct PlayerLobbyState : INetworkSerializable, IEquatable<PlayerLobbyState>
    {
        public ulong ClientId;
        public bool IsReady;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref IsReady);
        }

        public bool Equals(PlayerLobbyState other)
        {
            return ClientId == other.ClientId && IsReady == other.IsReady;
        }
    }

    public NetworkList<PlayerLobbyState> playerStates;

    public event Action OnPlayerListChanged;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        // 바로 이벤트 바인딩
        playerStates.OnListChanged += _ => OnPlayerListChanged?.Invoke();
    }

    [Rpc(SendTo.Server)]
    public void SetReadyServerRpc(ulong clientId)
    {
        for (int i = 0; i < playerStates.Count; i++)
        {
            if (playerStates[i].ClientId == clientId)
            {
                playerStates[i] = new PlayerLobbyState
                {
                    ClientId = clientId,
                    IsReady = true
                };
            }
        }
        OnPlayerListChanged?.Invoke();
    }

    public bool IsAllPlayersReady()
    {
        foreach (var state in playerStates)
        {
            if (!state.IsReady)
                return false;
        }
        return true;
    }

    public void LoadNextScene()
    {
        if (NetworkManager.Singleton.IsServer)
            NetworkManager.Singleton.SceneManager.LoadScene("BoardTest", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public void ApplyConnectionSettings(string ip)
    {
        ushort port = 7777; // default port

        //if (ushort.TryParse(portInputField.text, out ushort parsedPort))
        //    port = parsedPort;

        var unityTransport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();

        if (unityTransport != null)
        {
            unityTransport.SetConnectionData(ip, port);
        }
    }

    private void Awake()
    {
        playerStates = new NetworkList<PlayerLobbyState>();
    }


    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            playerStates.Add(new PlayerLobbyState
            {
                ClientId = clientId,
                IsReady = false
            });

            OnPlayerListChanged?.Invoke();
        }
    }

    
}
