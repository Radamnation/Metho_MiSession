using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class WebRequestBuilder
{
    private UnityWebRequest request;
    private string urlPrefix = "https://parseapi.back4app.com/";
    private string url;
    private string method = "POST";
    private string userSessionToken;
    private bool revocable;
    private string json;
    private string filePath;
    private string contentType;

    public WebRequestBuilder SetUrl(string _url)
    {
        url = urlPrefix + _url;
        return this;
    }

    public WebRequestBuilder SetMethod(string _method)
    {
        method = _method;
        return this;
    }

    public WebRequestBuilder SetSessionToken(string _userSessionToken)
    {
        userSessionToken = _userSessionToken;
        return this;
    }

    public WebRequestBuilder Revocable()
    {
        revocable = true;
        return this;
    }

    public WebRequestBuilder SetJSON(object _json)
    {
        json = JsonConvert.SerializeObject(_json);
        contentType = "application/json";
        return this;
    }

    public WebRequestBuilder SetPNG(string _filePath)
    {
        filePath = _filePath;
        contentType = "image/png";
        return this;
    }
    
    public WebRequestBuilder SetSaveFile(string _filePath)
    {
        filePath = _filePath;
        contentType = "data";
        return this;
    }

    public UnityWebRequest Build()
    {
        if (method == "PUT" && userSessionToken == null)
        {
            Debug.LogError("Session Token is required for PUT method");
            return null;
        }

        if (method == "GET" && !string.IsNullOrWhiteSpace(contentType))
        {
            Debug.LogError("Can't send information for GET method");
            return null;
        }
        
        request = new UnityWebRequest();
        request.url = url;
        request.method = method;
        request.SetRequestHeader("X-Parse-Application-Id", Secrets.ApplicationId);
        request.SetRequestHeader("X-Parse-REST-API-Key", Secrets.RestApiKey);
        
        if (revocable)
        {
            request.SetRequestHeader("X-Parse-Revocable-Session", "1");
        }

        if (!string.IsNullOrWhiteSpace(userSessionToken))
        {
            request.SetRequestHeader("X-Parse-Session-Token", userSessionToken);
        }

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            switch (contentType)
            {
                case "application/json":
                    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                    break;
                case "image/png":
                case "data":
                    request.uploadHandler = new UploadHandlerFile(filePath);
                    break;
                default:
                    Debug.LogError("Invalid content Type");
                    return null;
            }
            request.SetRequestHeader("Content-Type", contentType);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        return request;
    }
}