using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleView : UIView
{
    [SerializeField] private TMP_Text loginButtonText;
    [SerializeField] private TMP_Text playButtonText;
    [SerializeField] private TMP_Text currentLoginText;
    
    public override void OnShow()
    {
        base.OnShow();
        if (LoginManager.Instance.IsLoggedIn(out var userName))
        {
            loginButtonText.text = "Account";
            playButtonText.text = "Play";
            currentLoginText.text = "Currently logged as " + userName;
        }
        else
        {
            loginButtonText.text = "Login";
            playButtonText.text = "Play Offline";
            currentLoginText.text = "Not currently logged in";
        }
    }
}
