using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

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
    
    [SerializeField] private TMP_Text goldText;
    private SaveFile m_saveFile = new();
    private string m_filePath;
    
    private void Initialize()
    {
        m_filePath = Application.persistentDataPath + "/SaveFile.data";
        m_saveFile.gold = 100;
    }

    private void Start()
    {
        // goldText.text = saveFile.gold.ToString();
        // SaveGame(saveFile);

        m_saveFile = LoadGame();
        if (m_saveFile == null) return;
        goldText.text = m_saveFile.gold.ToString();
    }

    public void SaveGame(SaveFile _saveFile)
    {
        FileStream dataStream = new FileStream(m_filePath, FileMode.Create);

        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, _saveFile);

        dataStream.Close();
    }

    public SaveFile LoadGame()
    {
        if (!File.Exists(m_filePath))
        {
            Debug.Log("Save file not found in folder " + m_filePath);
            return null;
        }

        FileStream dataStream = new FileStream(m_filePath, FileMode.Open);

        BinaryFormatter converter = new BinaryFormatter();
        SaveFile saveFile = converter.Deserialize(dataStream) as SaveFile;

        dataStream.Close();
        return saveFile;
    }
}