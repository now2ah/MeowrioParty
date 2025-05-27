using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BoardScene : MonoBehaviour
{
    // [SerializeField] GameObject _leaderBoardManagerPrefab;
    // [SerializeField] GameObject _uiManagerPrefab;
    [SerializeField] GameObject _cameraManagerPrefab;
    [SerializeField] GameObject _boardManagerPrefab;
    [SerializeField] BoardManager _boardManager;

    private void Awake()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId, sceneName, loadSceneMode) =>
        {
            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                StartCoroutine(LoadManagersCoroutine());
            }
        };
    }

    private IEnumerator LoadManagersCoroutine()
    {
        LeaderBoardManager leaderBoardManager = null;
        UIManager uiManager = null;
        CameraManager cameraManager = null;
        //BoardManager boardManager = null;

        // if (_leaderBoardManagerPrefab != null)
        // {
        //     GameObject leaderBoardManagerObj = Instantiate(_leaderBoardManagerPrefab);
        //     if (leaderBoardManagerObj.TryGetComponent<LeaderBoardManager>(out LeaderBoardManager leaderBoardManagerComponent))
        //     {
        //         leaderBoardManager = leaderBoardManagerComponent;
        //     }
        // }

        // if (_uiManagerPrefab != null)
        // {
        //     GameObject uiManagerObj = Instantiate(_uiManagerPrefab);
        //     if (uiManagerObj.TryGetComponent<UIManager>(out UIManager uiManagerComponent))
        //     {
        //         uiManager = uiManagerComponent;
        //     }
        // }

        if (_cameraManagerPrefab != null)
        {
            GameObject cameraManagerObj = Instantiate(_cameraManagerPrefab);
            if (cameraManagerObj.TryGetComponent<CameraManager>(out CameraManager cameraManagerComponent))
            {
                cameraManager = cameraManagerComponent;
            }
        }

        while (cameraManager == null)
        {
            yield return null;
        }

        if (NetworkManager.Singleton.IsServer)
        {
            if (_boardManagerPrefab != null)
            {
                GameObject boardManagerObj = Instantiate(_boardManagerPrefab);
                if (boardManagerObj.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
                {
                    networkObject.Spawn();
                }
            }
        }
        LeaderBoardManager.Instance.InitializeLeaderBoard(NetworkManager.Singleton.ConnectedClientsList.Count);

        //if (_boardManager != null)
        //{
        //    if (_boardManager.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
        //    {
        //        networkObject.Spawn();
        //        _boardManager.gameObject.SetActive(true);
        //    }  
        //}
    }
}
