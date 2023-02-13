using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorMenu : MonoBehaviour
{
    [MenuItem("Tool/Create Scene Assets")]
    static void CreateSceneAssets()
    {
        foreach (var sceneAsset in GetSceneAssets())
        {
            var sceneData = ScriptableObject.CreateInstance<SceneData>();
            sceneData.SceneAsset = sceneAsset;
            var splitAssetPaths = AssetDatabase.GetAssetPath(sceneAsset.GetInstanceID()).Split("/");
            var assetPath = Path.Combine("Assets", "ScriptableObjects");
            for (int i = 2; i < splitAssetPaths.Length - 1; i++)
            {
                var folderName = splitAssetPaths[i];
                if (!AssetDatabase.IsValidFolder(Path.Combine(assetPath, folderName)))
                {
                    AssetDatabase.CreateFolder(assetPath, folderName);
                }
                assetPath = Path.Combine(assetPath, folderName);
            }
            assetPath = Path.Combine(assetPath, sceneData.SceneName + ".asset");
            AssetDatabase.CreateAsset(sceneData, assetPath);
        }
    }

    [MenuItem("Tool/Refresh Scene Assets")]
    static void RefreshSceneAssets()
    {
        foreach (var sceneData in GetSceneData())
        {
            sceneData.SceneAsset = sceneData.SceneAsset;
            EditorUtility.SetDirty(sceneData);
            string assetPath = AssetDatabase.GetAssetPath(sceneData.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, sceneData.SceneName);
            AssetDatabase.SaveAssets();
        }
    }

    static string[] sceneAssetsSearchInFolders = new[] { Path.Combine("Assets", "Scenes") };

    static List<SceneAsset> GetSceneAssets()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:SceneAsset", sceneAssetsSearchInFolders);
        var sceneAssets = new List<SceneAsset>();
        foreach (var sceneGuid in sceneGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(sceneGuid);
            sceneAssets.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath));
        }
        return sceneAssets;
    }

    static string[] sceneDataSearchInFolders = new[] { "Assets/ScriptableObjects/" };

    static List<SceneData> GetSceneData()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:SceneData", sceneDataSearchInFolders);
        var sceneDatas = new List<SceneData>();
        foreach (var sceneGuid in sceneGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(sceneGuid);
            sceneDatas.Add(AssetDatabase.LoadAssetAtPath<SceneData>(assetPath));
        }
        return sceneDatas;
    }
}
