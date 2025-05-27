using System;
using TMPro;
using UnityEngine.UI;

public class ExchangeStarUIData : BaseUIData
{
    public string DescTxt;
    public Action OnClickOKBtn;
    public Action OnClickCancelBtn;

}

public class ExchangeStarUI : BaseUI
{
    public TextMeshProUGUI DescTxt = null;


    public Button OKBtn = null;
    public Button CancelBtn = null;

    private ExchangeStarUIData m_ExchangeStarUIDate;
    private Action m_OnClickOKBtn;
    private Action m_OnClickCancelBtn;

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);

        m_ExchangeStarUIDate = uiData as ExchangeStarUIData;

        DescTxt.text = m_ExchangeStarUIDate.DescTxt;
        m_OnClickOKBtn = m_ExchangeStarUIDate.OnClickOKBtn;
        m_OnClickCancelBtn = m_ExchangeStarUIDate.OnClickCancelBtn;

    }

    public void onClickOkBtn()
    {
        m_OnClickOKBtn?.Invoke();
        m_OnClickOKBtn = null;
        CloseUI();
    }
    public void onClickCancleBtn()
    {
        m_OnClickCancelBtn?.Invoke();
        m_OnClickCancelBtn = null;
        CloseUI();
    }

}
