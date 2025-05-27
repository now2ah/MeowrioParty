using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUIData : BaseUIData
{
    public string[] StarCountTxt = new string[4];
    public string[] CoinCountTxt = new string[4];

    public Sprite[] PlayerSprs = new Sprite[4];
}

public class LeaderBoardUI : BaseUI
{
    public TextMeshProUGUI[] StarCountTxt = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] CoinCountTxt = new TextMeshProUGUI[4];

    public Image[] PlayerImgs = null;

    private LeaderBoardUIData m_leaderBoardUIData = null;
    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);

        m_leaderBoardUIData = uiData as LeaderBoardUIData;

        for (int i = 0; i < 2; i++) // 하드코딩 부분 나중에 수정 필요
        {
            StarCountTxt[i].text = m_leaderBoardUIData.StarCountTxt[i];
            CoinCountTxt[i].text = m_leaderBoardUIData.CoinCountTxt[i];
            PlayerImgs[i].sprite = m_leaderBoardUIData.PlayerSprs[i];
        }
        
    }
}
