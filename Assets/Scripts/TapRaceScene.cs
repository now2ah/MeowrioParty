using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TapRaceScene : MonoBehaviour
{
    [SerializeField] GameObject _miniGameManagerPrefab;

    private GameObject _miniGameManagerObject;

    private void Awake()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId, sceneName, loadSceneMode) =>
        {
            if (sceneName == "TapRaceScene")
            {
                if (NetworkManager.Singleton.LocalClientId == clientId)
                {
                    StartCoroutine(LoadManager());
                }
            }
        };

        //NetworkManager.Singleton.SceneManager.OnUnloadComplete += (clientId, sceneName) =>
        //{
        //    if (sceneName == "TapRaceScene")
        //    {
        //        if (NetworkManager.Singleton.LocalClientId == clientId)
        //        {
        //            StartCoroutine(UnLoadManager());
        //        }
        //    }
        //};
    }

    IEnumerator LoadManager()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            _miniGameManagerObject = Instantiate(_miniGameManagerPrefab);
            if (_miniGameManagerObject.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
            {
                networkObject.Spawn();
            }
        }
        yield return null;
    }

    IEnumerator UnLoadManager()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            _miniGameManagerObject = Instantiate(_miniGameManagerPrefab);
            if (_miniGameManagerObject.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
            {
                networkObject.Despawn();
            }
        }
        yield return null;
    }
}
