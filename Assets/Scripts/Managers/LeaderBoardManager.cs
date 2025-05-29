using System.Collections.Generic;
using UnityEngine;



public class LeaderBoardManager : Singleton<LeaderBoardManager>
{
    [SerializeField] DataSO dataSO;

    // private List<PlayerScores> playerScoreBoard = new();

    public void InitializeLeaderBoard(int size)
    {
        dataSO.playerScoreBoard.Clear();
        Debug.Log("leaderboard Initial : " + size);
        dataSO.clientCnt = size;
        for (int i = 0; i < dataSO.clientCnt; i++)
        {
            dataSO.playerScoreBoard.Add(new PlayerScores(i, 0, 0));
        }
    }
    public void UpdateCoin(ulong clientId, int addScore)
    {
        int index = (int)clientId;
        for (int i = 0; i < dataSO.playerScoreBoard.Count; i++)
        {
            if (dataSO.playerScoreBoard[i].playerId == index)
            {
                dataSO.playerScoreBoard[i].Coins += addScore;
            }
        }
        dataSO.OnScoreChanged();
    }
    public void UpdateStar(ulong clientId, int addScore)
    {
        int index = (int)clientId;
        for (int i = 0; i < dataSO.playerScoreBoard.Count; i++)
        {
            if (dataSO.playerScoreBoard[i].playerId == index)
            {
                dataSO.playerScoreBoard[i].Stars += addScore;
            }
        }
        dataSO.OnScoreChanged();
    }

    public void OpenLeaderBoardClient(LeaderBoardUIData lbData)
    {
        UIManager.Instance.OpenLeaderBoardUI(lbData);
        StartCoroutine(UIManager.Instance.CloseTargetUISecCo<LeaderBoardUI>(6f));
    }

    public int GetHighestScoreClientId()
    {
        return dataSO.playerScoreBoard[0].playerId;
    }


    public bool IsAffordStar(ulong clientId)
    {
        PlayerScores playerScore = null;

        foreach (var ps in dataSO.playerScoreBoard)
        {
            if (ps.playerId == (int)clientId)
                playerScore = ps;
        }

        if (playerScore.Coins >= 20)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

