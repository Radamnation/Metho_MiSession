using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginView : UIView
{
    [SerializeField] private TMP_Text loginInformationText;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    public override void OnShow()
    {
        base.OnShow();
        
        loginInformationText.enabled = false;
        loginInformationText.text = "";
        emailInputField.text = "";
        passwordInputField.text = "";
    }
    
    public void WriteError(string _string)
    {
        loginInformationText.text = _string;
        loginInformationText.enabled = true;
        loginInformationText.color = Color.red;
    }
    
    public void WriteSuccess(string _string)
    {
        loginInformationText.text = _string;
        loginInformationText.enabled = true;
        loginInformationText.color = Color.green;
    }

    public void CreateAccount()
    {
        LoginManager.Instance.CreateAccount(emailInputField.text, passwordInputField.text);
    }
    
    public void LoginAccount()
    {
        LoginManager.Instance.LoginAccount(emailInputField.text, passwordInputField.text);
    }
}
