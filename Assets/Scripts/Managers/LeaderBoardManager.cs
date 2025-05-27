using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScores
{
    public int playerId;
    public int Coins;
    public int Stars;

    public PlayerScores(int id, int a, int b)
    {
        playerId = id;
        Coins = a;
        Stars = b;
    }
}

public class LeaderBoardManager : Singleton<LeaderBoardManager>
{
    private int clientCnt = 0;

    private List<PlayerScores> playerScoreBoard = new();
    [SerializeField] private Sprite[] _playerPortraits;

    public void InitializeLeaderBoard(int size)
    {
        clientCnt = size;
        for (int i = 0; i < size; i++)
        {
            playerScoreBoard.Add(new PlayerScores(i, 0, 0));
        }
    }
    public void UpdateCoin(ulong clientId, int addScore)
    {
        int index = (int)clientId;
        playerScoreBoard[index].Coins += addScore;

        Debug.Log("afterCoin : " + playerScoreBoard[index].Coins);
    }
    public void UpdateStar(ulong clientId, int addScore)
    {
        int index = (int)clientId;
        playerScoreBoard[index].Stars += addScore;

        Debug.Log("afterStar : " + playerScoreBoard[index].Stars);
    }
    private List<PlayerScores> OrderingLeaderBoardClient()
    {
        return playerScoreBoard
            .OrderByDescending(p => p.Stars)
            .ThenByDescending(p => p.Coins)
            .ToList();
    }

    public void UpdateLeaderBoardClient()
    {
        playerScoreBoard = OrderingLeaderBoardClient();
        string[] starResult = new string[clientCnt];
        string[] coinResult = new string[clientCnt];
        Sprite[] sprResult = new Sprite[clientCnt];
        for (int i = 0; i < clientCnt; i++)
        {
            PlayerScores ps = playerScoreBoard[i];
            starResult[i] = ps.Stars.ToString();
            coinResult[i] = ps.Coins.ToString();
            sprResult[i] = _playerPortraits[ps.playerId];
        }
        LeaderBoardUIData lbData = new LeaderBoardUIData
        {
            StarCountTxt = starResult,
            CoinCountTxt = coinResult,
            PlayerSprs = sprResult
            // FirstPlayerSpr ~ FourthPlayerSpr는 필요 시 설정
        };

        UIManager.Instance.OpenLeaderBoardUI(lbData);
        StartCoroutine(UIManager.Instance.CloseTargetUISecCo<LeaderBoardUI>(10f));
    }

    // public void ResetLeaderboard()
    // {
    //     playerScores.Clear();
    //     UpdateLeaderBoardClient(new string[0]);
    // }
}

