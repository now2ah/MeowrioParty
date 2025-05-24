using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : NetworkBehaviour
{
    [SerializeField] private Transform[] _miniGameStartPos;
    [SerializeField] private Transform _miniGameFinishPos;
    [SerializeField] private GameObject _miniGamePlayerPrefab;
    private ulong _winnerClientId;
    private bool _isFinished = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        StartCoroutine(SpawnAllPlayers());
    }


    private IEnumerator SpawnAllPlayers()
    {
        // 모든 클라이언트가 연결될 때까지 대기
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count >= _miniGameStartPos.Length);

        int index = 0;
        foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
        {
            ulong clientId = kvp.Key;
            Vector3 pos = _miniGameStartPos[index].position;
            Quaternion rot = _miniGameStartPos[index].rotation;

            GameObject obj = Instantiate(_miniGamePlayerPrefab, pos, rot);
            NetworkObject netObj = obj.GetComponent<NetworkObject>();

            netObj.SpawnAsPlayerObject(clientId, true);

            var playerController = obj.GetComponent<MiniGamePlayerController>();
            Vector3 finishPos = _miniGameFinishPos.position;
            playerController.SetFinishPosition(finishPos);

            index++;
        }
    }

    // 서버에서 호출됨
    public void OnPlayerFinished(ulong clientId)
    {
        if (_isFinished) return;

        _isFinished = true;
        _winnerClientId = clientId;

        Debug.Log("우승자: " + clientId);

        StartCoroutine(CleanupMiniGame());
    }

    private IEnumerator CleanupMiniGame()
    {
        // 미니게임 플레이어 제거해야 됨       

        // 씬 언로드
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync("TapRaceScene");
        yield return unloadOp;

        Debug.Log("미니게임 씬 언로드 완료");
    }
}