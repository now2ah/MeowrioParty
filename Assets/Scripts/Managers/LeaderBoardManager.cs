using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardManager : NetSingleton<LeaderBoardManager>
{
    private List<PlayerData> playerScores = new List<PlayerData>();

    public override void Awake()
    {
        base.Awake();

        if (NetworkManager.Singleton.IsServer)
        {
            var networkObject = GetComponent<NetworkObject>();
            networkObject.Spawn(true);      // NetworkObject가 부착된 BoardManager가 부착된 게임오브젝트 스폰
        }
    }

    public void AddPlayer(ulong clientId, string playerName)
    {
        if (!IsServer) return;
        if (playerScores.Any(p => p.ClientId == clientId)) return;

        playerScores.Add(new PlayerData
        {
            ClientId = clientId,
            PlayerName = playerName,
            Coins = 0,
            Stars = 0
        });
    }

    public void UpdateCoin(ulong clientId, int addScore)
    {
        var player = playerScores.FirstOrDefault(p => p.ClientId == clientId);
        if (player != null)
        {
            player.Coins += addScore;
            playerScores = playerScores.OrderByDescending(p => p.Coins).ToList();
        }
        Debug.Log("afterCoin : " + player.Coins);
    }

    private string[] GetPlayerScoresNames()
    {
        string[] result = new string[playerScores.Count];

        for (int i = 0; i < playerScores.Count; i++)
        {
            PlayerData player = playerScores[i];
            result[i] = player.PlayerName + " 코인은 (" + player.Coins + ")";
        }

        return result;
    }


    public void UpdateLeaderBoardClient(string[] showPlayerData)
    {
        LeaderBoardUIData lbData = new LeaderBoardUIData
        {
            PlayerNameTxt = showPlayerData,
            // FirstPlayerSpr ~ FourthPlayerSpr는 필요 시 설정
        };

        UIManager.Instance.OpenLeaderBoardUI(lbData);
        StartCoroutine(UIManager.Instance.CloseFrontUISecCo(10f));
    }

    public void ShowLeaderBoardToAllClients()
    {
        UpdateLeaderBoardClient(GetPlayerScoresNames());
    }


    public void ResetLeaderboard()
    {
        playerScores.Clear();
        UpdateLeaderBoardClient(new string[0]);
    }
}

