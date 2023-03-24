using System;
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



public class Switch
{
    private Railwork test;
    private void Start()
    {
        test.StartAnimation(OnAnimationEnd);
    }
    
    private void OnAnimationEnd()
    {
        // sdkfsdkf
    }
}

public class Railwork
{
    private Action action;
    
    public void StartAnimation(Action callback)
    {
        // sdfjkhdsf
        // sdfjkhsdkjfh
        action = callback;
    }

    public void StopAnimation()
    {
        action();
    }
}
