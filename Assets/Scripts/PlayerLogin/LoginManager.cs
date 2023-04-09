using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    private string inputPassword;
    private string inputUsername;
    private string inputEmail;
    
    private string inputPromoCode;

    private string m_currentLogin = "";

    private string userObjectId = "";
    private string userSessionToken = "";
    private string username = "";
    private string imageName = "";
    private string imageUrl = "";
    private string saveFileName = "";
    private string saveFileUrl = "";
    
    public string Username => username;
    public string ImageUrl => imageUrl;
    public string SaveFileURL => saveFileUrl;

    private void Start()
    {
        // currentUserText.text = "Not currently logged in";
        
        // promoInformationText.enabled = false;
        // UpdatePortrait(defaultPortrait);
        // imageUploadButton.gameObject.SetActive(false);
    }

    public bool IsLoggedIn(out string _userName)
    {
        if (userSessionToken != "")
        {
            _userName = username;
            return true;
        }

        _userName = null;
        return false;
    }

    public void Logout()
    {
        userSessionToken = "";
    }

    public void CreateAccount(string _email, string _password)
    {
        inputUsername = inputEmail = _email;
        inputPassword = _password;
        StartCoroutine(PostAccount());
    }

    public void LoginAccount(string _email, string _password)
    {
        inputUsername = inputEmail = _email;
        inputPassword = _password;
        StartCoroutine(GetAccount());
    }

    public void UploadPortrait()
    {
        StartCoroutine(PostPortrait());
    }
    
    public void UploadSaveFile()
    {
        StartCoroutine(PostSaveFile());
    }

    public void ValidatePromoCode(string _promoCode)
    {
        if (m_currentLogin == "")
        {
            UIManager.Instance.AccountView.WriteError("Not currently logged in");
            return;
        }

        inputPromoCode = _promoCode;
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
                password = inputPassword,
                username = inputUsername,
                email = inputEmail
            };

            var json = JsonConvert.SerializeObject(data);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                var errorJObject = JObject.Parse(request.downloadHandler.text);
                var error = errorJObject["error"]?.ToString();
                UIManager.Instance.LoginView.WriteError(error);
                yield break;
            }

            UIManager.Instance.LoginView.WriteSuccess("Account has been created successfully");
            Debug.Log(request.downloadHandler.text);
        }
    }

    public IEnumerator GetAccount()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", inputEmail);
        form.AddField("password", inputPassword);

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
                var errorJObject = JObject.Parse(request.downloadHandler.text);
                var error = errorJObject["error"]?.ToString();
                UIManager.Instance.LoginView.WriteError(error);
                yield break;
            }

            UIManager.Instance.LoginView.WriteSuccess("Login Successful");
            m_currentLogin = request.downloadHandler.text;
            
            var userJObject = JObject.Parse(request.downloadHandler.text);
            userSessionToken = userJObject["sessionToken"]?.ToString();
            userObjectId = userJObject["objectId"]?.ToString();
            
            Debug.Log(request.downloadHandler.text);
            
            using (var newRequest = new UnityWebRequest($"https://parseapi.back4app.com/users/{userObjectId}", "GET"))
            {
                newRequest.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
                newRequest.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
                
                newRequest.downloadHandler = new DownloadHandlerBuffer();
                yield return newRequest.SendWebRequest();

                if (newRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(newRequest.downloadHandler.text);
                    yield break;
                }
                
                var newJObject = JObject.Parse(newRequest.downloadHandler.text);
                username = newJObject["username"]?.ToString();
                imageName = newJObject["imageName"]?.ToString();
                imageUrl = newJObject["imageUrl"]?.ToString();
                saveFileName = newJObject["saveFileName"]?.ToString();
                saveFileUrl = newJObject["saveFileUrl"]?.ToString();
            }

            UIManager.Instance.SwitchView(UIManager.Instance.AccountView);
        }
    }

    public IEnumerator PostPortrait()
    {
        using (var request = new UnityWebRequest("https://parseapi.back4app.com/files/portrait.png", "POST"))
        {
            request.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
            request.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
            request.SetRequestHeader("Content-Type", "image/png");
            string filePath = Path.Combine(Application.streamingAssetsPath, "portrait.png");

            request.uploadHandler = new UploadHandlerFile(filePath);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                yield break;
            }

            Debug.Log(request.downloadHandler.text);
            var imageJObject = JObject.Parse(request.downloadHandler.text);
            imageName = imageJObject["name"]?.ToString();
            imageUrl = imageJObject["url"]?.ToString();
            StartCoroutine(SavePortraitToAccount());
            UIManager.Instance.AccountView.UpdateImageVisual();
        }
    }

    public IEnumerator SavePortraitToAccount()
    {
        string json = JsonConvert.SerializeObject(new { imageName, imageUrl });
        using (var request = UnityWebRequest.Put($"https://parseapi.back4app.com/users/{userObjectId}", json))
        {
            request.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
            request.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
            request.SetRequestHeader("X-Parse-Session-Token", userSessionToken);
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            Debug.Log(request.downloadHandler.text);
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
        }
    }
    
    public IEnumerator PostSaveFile()
    {
        using (var request = new UnityWebRequest("https://parseapi.back4app.com/files/saveFile.data", "POST"))
        {
            request.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
            request.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
            request.SetRequestHeader("Content-Type", "data");
            string filePath = SaveManager.Instance.FilePath;

            request.uploadHandler = new UploadHandlerFile(filePath);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                yield break;
            }

            Debug.Log(request.downloadHandler.text);
            var imageJObject = JObject.Parse(request.downloadHandler.text);
            saveFileName = imageJObject["name"]?.ToString();
            saveFileUrl = imageJObject["url"]?.ToString();
            StartCoroutine(SaveSaveFileToAccount());
            UIManager.Instance.AccountView.UpdateImageVisual();
        }
    }
    
    public IEnumerator SaveSaveFileToAccount()
    {
        string json = JsonConvert.SerializeObject(new { saveFileName, saveFileUrl });
        using (var request = UnityWebRequest.Put($"https://parseapi.back4app.com/users/{userObjectId}", json))
        {
            request.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
            request.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
            request.SetRequestHeader("X-Parse-Session-Token", userSessionToken);
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            Debug.Log(request.downloadHandler.text);
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
        }
    }

    public IEnumerator PostPromoCode()
    {
        var promoCode = new
        {
            code = inputPromoCode,
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
                UIManager.Instance.AccountView.WriteError("Invalid Promo Code");
                Debug.Log(getRequest.error);
                yield break;
            }

            Debug.Log(getRequest.downloadHandler.text);
            var usernameMatch = Regex.Matches(getRequest.downloadHandler.text, "\"username\":\"(\\w+)",
                RegexOptions.Multiline);

            if (usernameMatch.Count == 0)
            {
                UIManager.Instance.AccountView.WriteError("Promo Code as already been used");
                yield break;
            }
            
            var objectIdMatch = Regex.Matches(getRequest.downloadHandler.text, "\"objectId\":\"(\\w+)",
                RegexOptions.Multiline);
            var objectId = objectIdMatch.First().Groups[1].Value;

            using (var postRequest = new UnityWebRequest(loginUrl + "/" + objectId, "PUT"))
            {
                postRequest.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
                postRequest.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
                postRequest.SetRequestHeader("Content-Type", "application/json");

                var currentUserMatch = Regex.Matches(m_currentLogin, "\"username\":\"(\\w+@\\w+.\\w+)",
                    RegexOptions.Multiline);
                var currentUser = currentUserMatch.First().Groups[1].Value;

                var data = new
                {
                    code = inputPromoCode,
                    username = currentUser
                };

                var json = JsonConvert.SerializeObject(data);
                postRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                postRequest.downloadHandler = new DownloadHandlerBuffer();

                yield return postRequest.SendWebRequest();

                if (postRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(postRequest.error);
                    yield break;
                }

                UIManager.Instance.AccountView.WriteSuccess("Promo Code was linked Successfully");
                Debug.Log(postRequest.downloadHandler.text);
            }
        }
    }
}