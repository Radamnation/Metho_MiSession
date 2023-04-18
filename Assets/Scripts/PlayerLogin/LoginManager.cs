using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEditor.PackageManager.Requests;
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

    public Dictionary<string, string> codesDictionary = new();

    private string inputPassword;
    private string inputUsername;
    private string inputEmail;
    private string inputPromoCode;

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
        SaveManager.Instance.CreateBlankSave();
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

    public void UploadSaveFile(Action _callback)
    {
        StartCoroutine(PostSaveFile(_callback));
    }

    public void ValidatePromoCode(string _promoCode)
    {
        inputPromoCode = _promoCode;

        if (inputPromoCode[0] == 'S' && SaveManager.Instance.SaveFile.extraSkinUnlocked ||
            inputPromoCode[0] == 'L' && SaveManager.Instance.SaveFile.extraLevelUnlocked ||
            inputPromoCode[0] == 'X' && SaveManager.Instance.SaveFile.doubleXPUnlocked)
        {
            UIManager.Instance.AccountView.WriteError("You've already unlocked this Promo Code");
            return;
        }

        if (!codesDictionary.ContainsKey(inputPromoCode))
        {
            UIManager.Instance.AccountView.WriteError("Invalid Promo Code");
            return;
        }

        StartCoroutine(PostPromoCode());

        if (inputPromoCode[0] == 'S')
        {
            SaveManager.Instance.SaveFile.extraSkinUnlocked = true;
        }

        if (inputPromoCode[0] == 'L')
        {
            SaveManager.Instance.SaveFile.extraLevelUnlocked = true;
        }

        if (inputPromoCode[0] == 'X')
        {
            SaveManager.Instance.SaveFile.doubleXPUnlocked = true;
        }

        SaveManager.Instance.SaveGame(null);
        UIManager.Instance.AccountView.UpdateVisual();
    }

    public IEnumerator PostAccount()
    {
        using (var request = new WebRequestBuilder()
                   .SetUrl("users")
                   .Revocable()
                   .SetJSON(new { password = inputPassword, username = inputUsername, email = inputEmail })
                   .Build())
        {
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
        string urlParameters = Encoding.UTF8.GetString(form.data);

        using (var request = new WebRequestBuilder()
                   .SetUrl($"login?{urlParameters}")
                   .SetMethod("GET")
                   .Revocable()
                   .Build())
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                var errorJObject = JObject.Parse(request.downloadHandler.text);
                var error = errorJObject["error"]?.ToString();
                UIManager.Instance.LoginView.WriteError(error);
                yield break;
            }

            var userJObject = JObject.Parse(request.downloadHandler.text);
            userSessionToken = userJObject["sessionToken"]?.ToString();
            userObjectId = userJObject["objectId"]?.ToString();
        }

        using (var request = new WebRequestBuilder()
                   .SetUrl($"users/{userObjectId}")
                   .SetMethod("GET")
                   .Build())
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                yield break;
            }

            var newJObject = JObject.Parse(request.downloadHandler.text);
            username = newJObject["username"]?.ToString();
            imageName = newJObject["imageName"]?.ToString();
            imageUrl = newJObject["imageUrl"]?.ToString();
            saveFileName = newJObject["saveFileName"]?.ToString();
            saveFileUrl = newJObject["saveFileUrl"]?.ToString();
        }

        StartCoroutine(GetPromoCode());
        UIManager.Instance.SwitchView(UIManager.Instance.AccountView);
    }

    public IEnumerator PostPortrait()
    {
        using (var request = new WebRequestBuilder()
                   .SetUrl("files/portrait.png")
                   .SetPNG(Path.Combine(Application.streamingAssetsPath, "portrait.png"))
                   .Build())
        {
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
        using (var request = new WebRequestBuilder()
                   .SetUrl($"users/{userObjectId}")
                   .SetMethod("PUT")
                   .SetSessionToken(userSessionToken)
                   .SetJSON(new { imageName, imageUrl })
                   .Build())
        {
            yield return request.SendWebRequest();

            Debug.Log(request.downloadHandler.text);

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
        }
    }

    public IEnumerator PostSaveFile(Action _callback)
    {
        using (var request = new WebRequestBuilder()
                   .SetUrl("files/saveFile.data")
                   .SetSaveFile(SaveManager.Instance.FilePath)
                   .Build())
        {
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
            StartCoroutine(SaveSaveFileToAccount(_callback));
        }
    }

    public IEnumerator SaveSaveFileToAccount(Action _callback)
    {
        using (var request = new WebRequestBuilder()
                   .SetUrl($"users/{userObjectId}")
                   .SetMethod("PUT")
                   .SetSessionToken(userSessionToken)
                   .SetJSON(new { saveFileName, saveFileUrl })
                   .Build())
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
        }

        _callback?.Invoke();
    }

    public IEnumerator GetPromoCode()
    {
        WWWForm form = new WWWForm();
        form.AddField("where", JsonConvert.SerializeObject(new { used = false }));
        string urlParameters = Encoding.UTF8.GetString(form.data);

        using (var request = new WebRequestBuilder()
                   .SetUrl("classes/PromoCode?" + urlParameters)
                   .SetMethod("GET")
                   .Build())
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                yield break;
            }

            codesDictionary.Clear();
            var jObjects = JObject.Parse(request.downloadHandler.text);
            var results = jObjects["results"]?.ToArray();
            foreach (var result in results)
            {
                var jObject = JObject.Parse(result.ToString());
                var code = jObject["code"]?.ToString();
                var objectId = jObject["objectId"]?.ToString();
                codesDictionary.Add(code, objectId);
            }
        }
    }

    public IEnumerator PostPromoCode()
    {
        using (var request = new WebRequestBuilder()
                   .SetUrl($"classes/PromoCode/{codesDictionary[inputPromoCode]}")
                   .SetMethod("PUT")
                   .SetSessionToken(userSessionToken)
                   .SetJSON(new { code = inputPromoCode, username, used = true })
                   .Build())
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                yield break;
            }

            UIManager.Instance.AccountView.WriteSuccess("Promo Code was linked Successfully");
            Debug.Log(request.downloadHandler.text);
        }
    }
}