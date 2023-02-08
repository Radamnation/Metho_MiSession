using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.UI;

public class MainView : UIView
{
    [SerializeField] private Image m_experienceBar;

    public void UpdateExperience()
    {
        var fillAmount = (float) Player.Instance.CurrentExperience / Player.Instance.NextLevel;
        m_experienceBar.fillAmount = fillAmount;
    }
}
