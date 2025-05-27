using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TapRaceScene : MonoBehaviour
{
    [SerializeField] GameObject _miniGameManagerPrefab;

    private void Awake()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId, sceneName, loadSceneMode) =>
        {
            if (sceneName == "TapRaceScene")
            {
                if (NetworkManager.Singleton.LocalClientId == clientId)
                {
                    StartCoroutine(LoadManagers());
                }
            }
        };
    }

    IEnumerator LoadManagers()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject miniGameManagerObj = Instantiate(_miniGameManagerPrefab);
            if (miniGameManagerObj.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
            {
                networkObject.Spawn();
            }
        }
        yield return null;
    }
}
