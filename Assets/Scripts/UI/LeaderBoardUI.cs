using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUIData : BaseUIData
{
    public string[] PlayerNameTxt = new string[4];

    public Sprite FirstPlayerSpr = null;
    public Sprite SecondPlayerSpr  = null;
    public Sprite ThirdPlayerISpr = null;
    public Sprite FourthPlayerSpr  = null; 
}

public class LeaderBoardUI : BaseUI
{
    // 닫기 버튼? 도 있어야 할 것 같습니다.
    public TextMeshProUGUI[] PlayerNameTxt = new TextMeshProUGUI[4];

    public Image FirstPlayerImg = null;
    public Image SecondPlayerImg = null;
    public Image ThirdPlayerImg = null;
    public Image FourthPlayerImg = null;

    private LeaderBoardUIData m_leaderBoardUIData = null;
    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);

        m_leaderBoardUIData = uiData as LeaderBoardUIData;

        for (int i = 0; i < 2; i++)
        {
            PlayerNameTxt[i].text = m_leaderBoardUIData.PlayerNameTxt[i];
        }
        
        FirstPlayerImg.sprite = m_leaderBoardUIData.FirstPlayerSpr;
        SecondPlayerImg.sprite = m_leaderBoardUIData.SecondPlayerSpr;
        ThirdPlayerImg.sprite = m_leaderBoardUIData.ThirdPlayerISpr;
        FourthPlayerImg.sprite = m_leaderBoardUIData.FourthPlayerSpr;
    }
}
