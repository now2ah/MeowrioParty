using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.Collections;
using Unity.Services.Matchmaker.Models;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    private NetworkList<PlayerLobbyState> _playerStates;

    public event Action OnPlayerListChanged;

    private void Awake()
    {
        Instance = this;

        _playerStates = new NetworkList<PlayerLobbyState>();
    }

    public override void OnDestroy()
    {
        // 최신 NetworkList는 Dispose 불필요하므로 제거 또는 단순 null 체크만
        _playerStates = null;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        _playerStates.OnListChanged += OnPlayerStatesChanged;
    }

    private void OnPlayerStatesChanged(NetworkListEvent<PlayerLobbyState> changeEvent)
    {
        NotifyPlayerListChanged();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            _playerStates.Add(new PlayerLobbyState
            {
                ClientId = clientId,
                IsReady = false
            });

            NotifyPlayerListChanged();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetReadyServerRpc(ulong clientId)
    {
        for (int i = 0; i < _playerStates.Count; i++)
        {
            if (_playerStates[i].ClientId == clientId)
            {
                _playerStates[i] = new PlayerLobbyState
                {
                    ClientId = clientId,
                    IsReady = true
                };
            }
        }

        NotifyPlayerListChanged();

        //if (AllPlayersReady())
        //{
        //    NetworkManager.Singleton.SceneManager.LoadScene("Board", LoadSceneMode.Single);
        //}
    }

    public bool AllPlayersReady()
    {
        foreach (var state in _playerStates)
        {
            if (!state.IsReady)
                return false;
        }
        return true;
    }

    private void NotifyPlayerListChanged()
    {
        OnPlayerListChanged?.Invoke();
    }

    public NetworkList<PlayerLobbyState> PlayerStates => _playerStates;

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

        public override bool Equals(object obj)
        {
            return obj is PlayerLobbyState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ClientId.GetHashCode() ^ IsReady.GetHashCode();
        }
    }
}
