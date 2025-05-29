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
    [SerializeField] private Sprite _defaultSprite;


    public void InitializeLeaderBoard(int size)
    {
        playerScoreBoard.Clear();
        Debug.Log("leaderboard Initial : " + size);
        clientCnt = size;
        for (int i = 0; i < clientCnt; i++)
        {
            playerScoreBoard.Add(new PlayerScores(i, 0, 0));
        }
    }
    public void UpdateCoin(ulong clientId, int addScore)
    {
        int index = (int)clientId;
        for (int i = 0; i < playerScoreBoard.Count; i++)
        {
            if (playerScoreBoard[i].playerId == index)
            {
                playerScoreBoard[i].Coins += addScore;
            }
        }
        UpdateLeaderBoardClient(false, false);
    }
    public void UpdateStar(ulong clientId, int addScore)
    {
        int index = (int)clientId;
        for (int i = 0; i < playerScoreBoard.Count; i++)
        {
            if (playerScoreBoard[i].playerId == index)
            {
                playerScoreBoard[i].Stars += addScore;
            }
        }
        UpdateLeaderBoardClient(false, false);
    }
    private List<PlayerScores> OrderingLeaderBoardClient()
    {
        return playerScoreBoard
            .OrderByDescending(p => p.Stars)
            .ThenByDescending(p => p.Coins)
            .ToList();
    }

    public void OpenLeaderBoardClient(LeaderBoardUIData lbData)
    {
        UIManager.Instance.OpenLeaderBoardUI(lbData);
        StartCoroutine(UIManager.Instance.CloseTargetUISecCo<LeaderBoardUI>(6f));
    }

        string[] starResult = new string[4];
        string[] coinResult = new string[4];
        Sprite[] sprResult = new Sprite[4];
        
    public void UpdateLeaderBoardClient(bool isFirst, bool isLBOpen)
    {
        playerScoreBoard = OrderingLeaderBoardClient();
        for (int i = 0; i < clientCnt; i++)
        {
            PlayerScores ps = playerScoreBoard[i];
            starResult[i] = ps.Stars.ToString();
            coinResult[i] = ps.Coins.ToString();
            sprResult[i] = _playerPortraits[ps.playerId];
        }
        for (int i = clientCnt; i < 4; i++)
        {
            sprResult[i] = _defaultSprite;
        }
        LeaderBoardUIData lbData = new LeaderBoardUIData
        {
            StarCountTxt = starResult,
            CoinCountTxt = coinResult,
            PlayerSprs = sprResult,
            isAnimPlay = !isFirst
            // FirstPlayerSpr ~ FourthPlayerSpr는 필요 시 설정
        };
        PlayerCurrentStateUIData pcData = new PlayerCurrentStateUIData
        {
            StarCountTxt = starResult,
            CoinCountTxt = coinResult,
            PlayerSprs = sprResult,
        };
        UIManager.Instance.CloseTargetUI<PlayerCurrentStateUI>();
        UIManager.Instance.OpenPlayerCurrentUI(pcData);

        if (isLBOpen) OpenLeaderBoardClient(lbData);
    }

    // public void ResetLeaderboard()
    // {
    //     playerScores.Clear();
    //     UpdateLeaderBoardClient(new string[0]);
    // }
}

