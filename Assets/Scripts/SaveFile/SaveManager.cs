using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System;
using PlasticGui.Configuration.CloudEdition;
using TMPro;
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

    public string FilePath => m_filePath;
    
    private void Initialize()
    {
        m_filePath = Application.persistentDataPath + "/SaveFile.data";
    }

    public void CreateBlankSave()
    {
        FileStream dataStream = new FileStream(m_filePath, FileMode.Create);

        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, new SaveFile());

        dataStream.Close();
    }

    public void SaveGame(SaveFile _saveFile)
    {
        FileStream dataStream = new FileStream(m_filePath, FileMode.Create);

        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, _saveFile);

        dataStream.Close();
        
        LoginManager.Instance.UploadSaveFile();
    }

    public void LoadGame()
    {
        saveFileUrl = LoginManager.Instance.SaveFileURL;
        if (!string.IsNullOrEmpty(saveFileUrl))
        {
            StartCoroutine(GetSaveFileFromURL());
        }
        else
        {
            SaveGame(m_saveFile);
        }
    }

    public IEnumerator GetSaveFileFromURL()
    {
        using (var request = UnityWebRequest.Get(LoginManager.Instance.SaveFileURL))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.Log(request.downloadHandler.text);
            }
            
            // var data = request.downloadHandler.data;
            File.WriteAllBytes(m_filePath, request.downloadHandler.data);
        }
        
        // if (!File.Exists(m_filePath))
        // {
        //     Debug.Log("Save file not found in folder " + m_filePath + ", creating blank Save File");
        //     CreateBlankSave();
        // }

        FileStream dataStream = new FileStream(m_filePath, FileMode.Open);

        BinaryFormatter converter = new BinaryFormatter();
        SaveFile saveFile = converter.Deserialize(dataStream) as SaveFile;

        dataStream.Close();
        m_saveFile = saveFile;
    }
}