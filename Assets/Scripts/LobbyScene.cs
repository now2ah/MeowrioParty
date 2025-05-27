using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private GameObject _networkManagerPrefab;
    [SerializeField] private GameObject _lobbyManagerPrefab;
    [SerializeField] private LobbyUI _lobbyUI;

    void Awake()
    {
        StartCoroutine(LoadManagersCoroutine());
    }

    IEnumerator LoadManagersCoroutine()
    {
        NetworkManager networkManager = null;

        if (_networkManagerPrefab != null)
        {
            GameObject networkManagerObj = Instantiate(_networkManagerPrefab);
            if (networkManagerObj.TryGetComponent<NetworkManager>(out NetworkManager networkManagerComponent))
            {
                networkManager = networkManagerComponent;
            }
        }

        while (networkManager == null)
        {
            yield return null;
        }

        if (_lobbyManagerPrefab != null)
        {
            GameObject lobbyManagerObj = Instantiate(_lobbyManagerPrefab);

            if (lobbyManagerObj.TryGetComponent<LobbyManager>(out LobbyManager lobbyManager))
            {
                if (_lobbyUI != null)
                {
                    _lobbyUI.LobbyManager = lobbyManager;
                }
            }
        }
    }
}
