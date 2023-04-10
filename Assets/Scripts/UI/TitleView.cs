using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TitleView : UIView
{
    [SerializeField] private TMP_Text loginButtonText1;
    [SerializeField] private TMP_Text loginButtonText2;
    [SerializeField] private TMP_Text playButtonText1;
    [SerializeField] private TMP_Text playButtonText2;
    [SerializeField] private TMP_Text currentLoginText;
    
    public override void OnShow()
    {
        base.OnShow();
        if (LoginManager.Instance.IsLoggedIn(out var userName))
        {
            loginButtonText1.gameObject.SetActive(false);
            loginButtonText2.gameObject.SetActive(true);
            
            playButtonText1.gameObject.SetActive(false);
            playButtonText2.gameObject.SetActive(true);
            
            currentLoginText.text = "Currently logged as " + userName;
        }
        else
        {
            loginButtonText1.gameObject.SetActive(true);
            loginButtonText2.gameObject.SetActive(false);
            
            playButtonText1.gameObject.SetActive(true);
            playButtonText2.gameObject.SetActive(false);
            
            currentLoginText.text = "Not currently logged in";
        }
    }
}
