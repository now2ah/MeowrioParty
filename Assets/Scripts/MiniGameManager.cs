using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : NetworkBehaviour
{
    [SerializeField] private Transform[] _miniGameStartPos;
    [SerializeField] private Transform _miniGameFinishPos;
    [SerializeField] private GameObject[] _miniGamePlayerPrefab;
    [SerializeField] private List<NetworkObject> _miniGameCharacterList;

    private void Awake()
    {
        _miniGameCharacterList = new List<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        StartCoroutine(SpawnAllPlayers());
    }

    private IEnumerator SpawnAllPlayers()
    {
        // 모든 클라이언트가 연결될 때까지 대기
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count >= BoardManager.Instance._playerCtrlMap.Count);

        int index = 0;
        foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
        {
            ulong clientId = kvp.Key;
            Vector3 pos = _miniGameStartPos[index].position;
            Quaternion rot = _miniGameStartPos[index].rotation;

            GameObject obj = Instantiate(_miniGamePlayerPrefab[index], pos, rot);
            NetworkObject netObj = obj.GetComponent<NetworkObject>();
            _miniGameCharacterList.Add(netObj);

            netObj.Spawn();
            netObj.ChangeOwnership(clientId);

            //netObj.SpawnAsPlayerObject(clientId, true);

            obj.transform.SetParent(gameObject.transform);

            var playerController = obj.GetComponent<MiniGamePlayerController>();
            Vector3 finishPos = _miniGameFinishPos.position;
            playerController.SetFinishPosition(finishPos);
            playerController.OnFinish += PlayerController_OnFinish;

            index++;
        }

        CameraManager.Instance.ChangeCamera(CameraType.MiniGame);
    }

    private void PlayerController_OnFinish()
    {
        foreach (var characters in _miniGameCharacterList)
        {
            characters.Despawn();
        }

        if (IsServer)
        {
            NetworkObject netObj = GetComponent<NetworkObject>();
            netObj.Despawn();
        }
    }
}