using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainView : UIView
{
    [SerializeField] private Image m_experienceBar;
    [SerializeField] private TMP_Text m_goldText;
    [SerializeField] private TMP_Text m_timerText;
    [SerializeField] private TMP_Text m_levelText;

    public override void OnShow()
    {
        base.OnShow();
        UpdateGold();
    }

    public void UpdateExperience()
    {
        var fillAmount = (float) Player.Instance.CurrentExperience / Player.Instance.NextLevel;
        m_experienceBar.fillAmount = fillAmount;
    }

    public void UpdateGold()
    {
        m_goldText.text = "GOLD || " + SaveManager.Instance.SaveFile.gold;
    }

    public void UpdateTimer()
    {
        var time = Player.Instance.TimeSurvived;
        int minutes =  (int) time / 60;
        int seconds = (int) time % 60;
        m_timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void UpdateLevel()
    {
        m_levelText.text = Player.Instance.CurrentLevel + " || LVL";
    }
}
