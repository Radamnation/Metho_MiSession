using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : ScriptableObject
{
    public string SceneName;

#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset sceneAsset;

    public UnityEditor.SceneAsset SceneAsset
    {
        get => sceneAsset;
        set
        {
            sceneAsset = value;
            SceneName = sceneAsset.name;
        }
    }
#endif
}
