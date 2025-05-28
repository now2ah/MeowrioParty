using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCurrentStateUIData : BaseUIData
{
    public string[] StarCountTxt = new string[4];
    public string[] CoinCountTxt = new string[4];

    public Sprite[] PlayerSprs = new Sprite[4];
}

public class PlayerCurrentStateUI : BaseUI
{
    public TextMeshProUGUI[] StarCountTxt = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] CoinCountTxt = new TextMeshProUGUI[4];

    public Image[] PlayerImgs = null;

    private PlayerCurrentStateUIData m_playerCurrentStateUIData = null;
    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);

        m_playerCurrentStateUIData = uiData as PlayerCurrentStateUIData;

        for (int i = 0; i < 4; i++) // 하드코딩 부분 나중에 수정 필요
        {
            StarCountTxt[i].text = m_playerCurrentStateUIData.StarCountTxt[i];
            CoinCountTxt[i].text = m_playerCurrentStateUIData.CoinCountTxt[i];
            PlayerImgs[i].sprite = m_playerCurrentStateUIData.PlayerSprs[i];
        }
        
    }
}
