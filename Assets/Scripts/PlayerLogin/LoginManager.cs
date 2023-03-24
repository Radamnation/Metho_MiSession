using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_Text currentUserText;
    
    [SerializeField] private TMP_Text loginInformationText;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    
    [SerializeField] private TMP_Text promoInformationText;
    [SerializeField] private TMP_InputField promoCodeInputField;

    private string m_currentLogin = "";

    private void Awake()
    {
        currentUserText.text = "Not currently logged in";
        
        loginInformationText.enabled = false;
        loginInformationText.text = "";
        
        promoInformationText.enabled = false;
        loginInformationText.text = "";
    }

    public void CreateAccount()
    {
        StartCoroutine(PostAccount());
    }

    public void LoginAccount()
    {
        StartCoroutine(GetAccount(UpdateVisual));
    }

    private void UpdateVisual()
    {
        if (m_currentLogin == "")
        {
            currentUserText.text = "Not currently logged in";
            return;
        }
        var usernameMatch = Regex.Matches(m_currentLogin, "\"username\":\"(\\w+)", RegexOptions.Multiline);
        var username = usernameMatch.First().Groups[1].Value;
        currentUserText.text = "Currently logged in as " + username;
    }
    
    public void ValidatePromoCode()
    {
        if (m_currentLogin == "")
        {
            promoInformationText.enabled = true;
            promoInformationText.text = "Not currently logged in";
            promoInformationText.color = Color.red;
            return;
        }
        StartCoroutine(PostPromoCode());
    }
    
    public IEnumerator PostAccount()
    {
        using (var request = new UnityWebRequest("https://parseapi.back4app.com/users", "POST"))
        {
            request.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
            request.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
            request.SetRequestHeader("X-Parse-Revocable-Session", "1");
            request.SetRequestHeader("Content-Type", "application/json");

            var data = new
            {
                password = passwordInputField.text,
                username = emailInputField.text,
                email = emailInputField.text
            };
            
            var json = JsonConvert.SerializeObject(data);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                loginInformationText.enabled = true;
                loginInformationText.text = "Impossible to create account";
                loginInformationText.color = Color.red;
                Debug.LogError(request.error);
                yield break;
            }

            loginInformationText.enabled = true;
            loginInformationText.text = "Account has been created successfully";
            loginInformationText.color = Color.green;
            Debug.Log(request.downloadHandler.text);
        }
    }
    
    public IEnumerator GetAccount(Action _callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", emailInputField.text);
        form.AddField("password", passwordInputField.text);
        
        string loginUrl = "https://parseapi.back4app.com/login";
        string loginUrlWithParams = loginUrl + "?" + Encoding.UTF8.GetString(form.data);

        using (var request = new UnityWebRequest(loginUrlWithParams, "GET"))
        {
            request.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
            request.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
            request.SetRequestHeader("X-Parse-Revocable-Session", "1");

            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                loginInformationText.enabled = true;
                loginInformationText.text = "Incorrect Email or Password";
                loginInformationText.color = Color.red;
                Debug.LogError(request.error);
                yield break;
            }

            loginInformationText.enabled = true;
            loginInformationText.text = "Login Successful";
            loginInformationText.color = Color.green;
            m_currentLogin = request.downloadHandler.text;
            Debug.Log(request.downloadHandler.text);
            _callback?.Invoke();
        }
    }
    
    public IEnumerator PostPromoCode()
    {
        var promoCode = new
        {
            code = promoCodeInputField.text,
            username = "null"
        };
        
        WWWForm form = new WWWForm();
        form.AddField("where", JsonConvert.SerializeObject(promoCode));
        
        string loginUrl = "https://parseapi.back4app.com/classes/PromoCode";
        string loginUrlWithParams = loginUrl + "?" + Encoding.UTF8.GetString(form.data);
        Debug.Log(loginUrlWithParams);

        using (var getRequest = new UnityWebRequest(loginUrlWithParams, "GET"))
        {
            getRequest.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
            getRequest.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
            
            getRequest.downloadHandler = new DownloadHandlerBuffer();
            
            yield return getRequest.SendWebRequest();

            if (getRequest.result != UnityWebRequest.Result.Success)
            {
                promoInformationText.enabled = true;
                promoInformationText.text = "Invalid or already used PromoCode";
                promoInformationText.color = Color.red;
                Debug.LogError(getRequest.error);
                yield break;
            }
            
            Debug.Log(getRequest.downloadHandler.text);
            var usernameMatch = Regex.Matches(getRequest.downloadHandler.text, "\"username\":\"(\\w+)", RegexOptions.Multiline);
            
            if (usernameMatch.Count == 0)
            {
                promoInformationText.enabled = true;
                promoInformationText.text = "PromoCode as already been used";
                promoInformationText.color = Color.red;
                yield break;
            }
            
            // var username = usernameMatch.First().Groups[1].Value;
            
            var objectIdMatch = Regex.Matches(getRequest.downloadHandler.text, "\"objectId\":\"(\\w+)", RegexOptions.Multiline);
            var objectId = objectIdMatch.First().Groups[1].Value;

            using (var postRequest = new UnityWebRequest(loginUrl + "/" + objectId, "PUT"))
            {
                postRequest.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
                postRequest.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
                postRequest.SetRequestHeader("Content-Type", "application/json");
                
                var currentUserMatch = Regex.Matches(m_currentLogin, "\"username\":\"(\\w+@\\w+.\\w+)", RegexOptions.Multiline);
                var currentUser = currentUserMatch.First().Groups[1].Value;
                
                var data = new
                {
                    code = promoCodeInputField.text,
                    username = currentUser
                };
            
                var json = JsonConvert.SerializeObject(data);
                postRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                postRequest.downloadHandler = new DownloadHandlerBuffer();

                yield return postRequest.SendWebRequest();

                if (postRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(postRequest.error);
                    yield break;
                }

                promoInformationText.enabled = true;
                promoInformationText.text = "PromoCode was linked Successfully";
                promoInformationText.color = Color.green;
                Debug.Log(postRequest.downloadHandler.text);
            }
        }
    }
}
