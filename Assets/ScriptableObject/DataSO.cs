using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[SerializeField]
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


[CreateAssetMenu(fileName = "DataSO", menuName = "Scriptable Objects/DataSO")]
public class DataSO : ScriptableObject
{
    public List<PlayerScores> playerScoreBoard = new();
    public int clientCnt = 0;
    public bool isFirst = false;
    public bool isLBOpen = false;
    [SerializeField] private Sprite[] _playerPortraits;
    [SerializeField] private Sprite _defaultSprite;


    public Action onScoreChangedAction;

    private void OnEnable()
    {
        onScoreChangedAction += UpdateLeaderBoardClient;
    }
    private void OnDisable()
    {
        onScoreChangedAction -= UpdateLeaderBoardClient;
    }
    public void OnScoreChanged()
    {
        onScoreChangedAction?.Invoke();
    }


    string[] starResult = new string[4];
    string[] coinResult = new string[4];
    Sprite[] sprResult = new Sprite[4];

    public void UpdateLeaderBoardClient() //score 바뀔 때마다 갱신해주는 함수 
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
            coinResult[i] = 0.ToString();
        }
        PlayerCurrentStateUIData pcData = new PlayerCurrentStateUIData
        {
            StarCountTxt = starResult,
            CoinCountTxt = coinResult,
            PlayerSprs = sprResult,
        };

        UIManager.Instance.CloseTargetUI<PlayerCurrentStateUI>();
        UIManager.Instance.OpenPlayerCurrentUI(pcData);
    }
    public void OpenLeaderBoard() //리더보드 띄우는 함수
    {
        LeaderBoardUIData lbData = new LeaderBoardUIData
        {
            StarCountTxt = starResult,
            CoinCountTxt = coinResult,
            PlayerSprs = sprResult,
            isAnimPlay = !isFirst
            // FirstPlayerSpr ~ FourthPlayerSpr는 필요 시 설정
        };
        LeaderBoardManager.Instance.OpenLeaderBoardClient(lbData);
    }
    public List<PlayerScores> OrderingLeaderBoardClient()
    {
        return playerScoreBoard
            .OrderByDescending(p => p.Stars)
            .ThenByDescending(p => p.Coins)
            .ToList();
    }
}
