using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.Collections;
using Unity.Services.Matchmaker.Models;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    public NetworkList<PlayerLobbyState> playerStates;

    public event Action OnPlayerListChanged;

    private void Awake()
    {
        Instance = this;

        playerStates = new NetworkList<PlayerLobbyState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        // 바로 이벤트 바인딩
        playerStates.OnListChanged += _ => OnPlayerListChanged?.Invoke();
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
