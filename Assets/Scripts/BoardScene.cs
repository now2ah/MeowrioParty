using Meowrio.Util;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BoardScene : MonoBehaviour
{
    [SerializeField] GameObject _cameraManagerPrefab;
    [SerializeField] GameObject _boardGameManagerPrefab;
    [SerializeField] GameObject _inputHandlerPrefab;

    private void Awake()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId, sceneName, loadSceneMode) =>
        {
            if (sceneName == "BoardTest")
            {
                if (NetworkManager.Singleton.LocalClientId == clientId)
                {
                    StartCoroutine(LoadManagersCoroutine());
                }
            }
        };
    }

    private IEnumerator LoadManagersCoroutine()
    {
        //CameraManager cameraManager = null;

        //if (_cameraManagerPrefab != null)
        //{
        //    GameObject cameraManagerObj = Instantiate(_cameraManagerPrefab);
        //    if (cameraManagerObj.TryGetComponent<CameraManager>(out CameraManager cameraManagerComponent))
        //    {
        //        cameraManager = cameraManagerComponent;
        //    }
        //}

        
        if (NetworkManager.Singleton.IsServer)
        {
            if (_boardGameManagerPrefab != null)
            {
                GameObject boardGameManagerObj = Instantiate(_boardGameManagerPrefab);
                if (boardGameManagerObj.TryGetComponent<NetworkObject>(out NetworkObject boardGameManagerNetworkObject))
                {
                    boardGameManagerNetworkObject.Spawn();
                }
            }

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (_inputHandlerPrefab != null)
                {
                    GameObject inputHandlerObj = Instantiate(_inputHandlerPrefab);
                    if (inputHandlerObj.TryGetComponent<NetworkObject>(out NetworkObject inputHandlerNetworkObject))
                    {
                        inputHandlerNetworkObject.SpawnAsPlayerObject(client.ClientId);
                    }
                }
            }
        }

        



        //LeaderBoardManager.Instance.InitializeLeaderBoard(NetworkManager.Singleton.ConnectedClientsList.Count);

        yield return null;
    }
}
