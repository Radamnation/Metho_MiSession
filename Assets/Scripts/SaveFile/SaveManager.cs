using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
            return;
        }
        Destroy(gameObject);
    }
    
    private SaveFile m_saveFile = new();
    private string m_filePath;

    private string saveFileUrl;

    public SaveFile SaveFile => m_saveFile;
    public string FilePath => m_filePath;
    
    private void Initialize()
    {
        m_filePath = Application.persistentDataPath + "/SaveFile.data";
    }

    public void CreateBlankSave()
    {
        m_saveFile = new();
    }

    public void SaveGame(Action _callback)
    {
        FileStream dataStream = new FileStream(m_filePath, FileMode.Create);

        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, m_saveFile);

        dataStream.Close();
        
        LoginManager.Instance.UploadSaveFile(_callback);
    }

    public void LoadGame(Action _callback)
    {
        saveFileUrl = LoginManager.Instance.SaveFileURL;
        Debug.Log(saveFileUrl);
        if (!string.IsNullOrEmpty(saveFileUrl))
        {
            Debug.Log("Fetching Saved File");
            StartCoroutine(GetSaveFileFromURL(_callback));
        }
        else
        {
            Debug.Log("Saving new Save File");
            SaveGame(_callback);
        }
    }

    public IEnumerator GetSaveFileFromURL(Action _callback)
    {
        using (var request = UnityWebRequest.Get(LoginManager.Instance.SaveFileURL))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.Log(request.downloadHandler.text);
            }
            
            File.WriteAllBytes(m_filePath, request.downloadHandler.data);
        }

        FileStream dataStream = new FileStream(m_filePath, FileMode.Open);

        BinaryFormatter converter = new BinaryFormatter();
        SaveFile saveFile = converter.Deserialize(dataStream) as SaveFile;

        dataStream.Close();
        m_saveFile = saveFile;
        
        _callback?.Invoke();
    }
}