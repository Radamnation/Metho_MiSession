using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountView : UIView
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private TMP_Text unlockText;
    
    [SerializeField] private TMP_Text currentLoginText;
    
    [SerializeField] private TMP_Text promoInformationText;
    [SerializeField] private TMP_InputField promoCodeInputField;
    
    [SerializeField] private Image portraitImage;
    [SerializeField] private Texture2D defaultPortrait;

    private string imageUrl = "";
    
    public override void OnShow()
    {
        base.OnShow();
        
        var username = LoginManager.Instance.Username;
        usernameText.text = username;
        currentLoginText.text = "Currently logged as " + username;

        promoInformationText.text = "";
        promoCodeInputField.text = "";
        
        UpdateImageVisual();

        SaveManager.Instance.LoadGame(UpdateVisual);
    }

    public void UpdateVisual()
    {
        goldText.text = "Gold Amount : " + SaveManager.Instance.SaveFile.gold + " G";
        var time = SaveManager.Instance.SaveFile.bestTime;
        int minutes =  (int) time / 60;
        int seconds = (int) time % 60;
        bestTimeText.text = "Best Time : " + minutes.ToString("00") + ":" + seconds.ToString("00");
        unlockText.text = "Unlocks : ";
        if (SaveManager.Instance.SaveFile.extraSkinUnlocked)
        {
            unlockText.text += "| Skin | ";
        }
        if (SaveManager.Instance.SaveFile.extraLevelUnlocked)
        {
            unlockText.text += "| Level | ";
        }
        if (SaveManager.Instance.SaveFile.doubleXPUnlocked)
        {
            unlockText.text += "| 2XP |";
        }
    }

    public void Logout()
    {
        LoginManager.Instance.Logout();
        UIManager.Instance.SwitchToTitleScreen();
    }
    
    public void WriteError(string _string)
    {
        promoInformationText.text = _string;
        promoInformationText.enabled = true;
        promoInformationText.color = Color.red;
    }
    
    public void WriteSuccess(string _string)
    {
        promoInformationText.text = _string;
        promoInformationText.enabled = true;
        promoInformationText.color = Color.green;
    }
    
    private void UpdatePortrait(Texture2D _texture)
    {
        var sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f),
            100f);
        portraitImage.sprite = sprite;
    }
    
    public void UpdateImageVisual()
    {
        imageUrl = LoginManager.Instance.ImageUrl;
        if (!string.IsNullOrEmpty(imageUrl))
        {
            StartCoroutine(GetPortraitFromURL());
        }
        else
        {
            UpdatePortrait(defaultPortrait);
        }
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

    public void ValidatePromoCode()
    {
        LoginManager.Instance.ValidatePromoCode(promoCodeInputField.text);
    }

    public void UploadPortrait()
    {
        LoginManager.Instance.UploadPortrait();
    }
}
