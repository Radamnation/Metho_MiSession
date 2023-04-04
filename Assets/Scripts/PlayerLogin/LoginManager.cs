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
    [SerializeField] private TMP_Text currentUserText;

    [SerializeField] private TMP_Text loginInformationText;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] private TMP_Text promoInformationText;
    [SerializeField] private TMP_InputField promoCodeInputField;

    [SerializeField] private Image portraitImage;
    [SerializeField] private Texture2D defaultPortrait;

    [SerializeField] private Button imageUploadButton;

    private string m_currentLogin = "";

    private string userObjectId = "";
    private string userSessionToken = "";
    private string userName = "";
    private string imageName = "";
    private string imageUrl = "";

    private void Start()
    {
        currentUserText.text = "Not currently logged in";

        loginInformationText.enabled = false;
        loginInformationText.text = "";

        // promoInformationText.enabled = false;
        loginInformationText.text = "";

        // UpdatePortrait(defaultPortrait);
        // imageUploadButton.gameObject.SetActive(false);
    }

    private void UpdatePortrait(Texture2D _texture)
    {
        var sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f),
            100f);
        portraitImage.sprite = sprite;
    }

    public void CreateAccount()
    {
        StartCoroutine(PostAccount());
    }

    public void LoginAccount()
    {
        StartCoroutine(GetAccount(UpdateVisual));
    }

    public void UploadPortrait()
    {
        StartCoroutine(PostPortrait());
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

        imageUploadButton.gameObject.SetActive(true);
        if (imageUrl != "")
        {
            StartCoroutine(GetPortraitFromURL());
        }
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

    public IEnumerator GetPortraitFromURL()
    {
        using (var request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.Log(request.downloadHandler.text);
                yield break;
            }

            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            UpdatePortrait(texture);
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
                var errorJObject = JObject.Parse(request.downloadHandler.text);
                var error = errorJObject["error"]?.ToString();
                loginInformationText.text = error;
                loginInformationText.enabled = true;
                loginInformationText.color = Color.red;
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
                var errorJObject = JObject.Parse(request.downloadHandler.text);
                var error = errorJObject["error"]?.ToString();
                loginInformationText.text = error;
                loginInformationText.enabled = true;
                loginInformationText.color = Color.red;
                yield break;
            }

            loginInformationText.enabled = true;
            loginInformationText.text = "Login Successful";
            loginInformationText.color = Color.green;
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
                imageName = newJObject["imageName"]?.ToString();
                imageUrl = newJObject["imageUrl"]?.ToString();
                UpdateVisual();
            }
            
            _callback?.Invoke();
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
            UpdateVisual();
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
                promoInformationText.text = "Invalid Promo Code";
                promoInformationText.color = Color.red;
                Debug.Log(getRequest.error);
                yield break;
            }

            Debug.Log(getRequest.downloadHandler.text);
            var usernameMatch = Regex.Matches(getRequest.downloadHandler.text, "\"username\":\"(\\w+)",
                RegexOptions.Multiline);

            if (usernameMatch.Count == 0)
            {
                promoInformationText.enabled = true;
                promoInformationText.text = "Promo Code as already been used";
                promoInformationText.color = Color.red;
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
                    code = promoCodeInputField.text,
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

                promoInformationText.enabled = true;
                promoInformationText.text = "Promo Code was linked Successfully";
                promoInformationText.color = Color.green;
                Debug.Log(postRequest.downloadHandler.text);
            }
        }
    }
}