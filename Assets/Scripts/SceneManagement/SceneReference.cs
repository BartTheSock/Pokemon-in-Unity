using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneReference : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    [SerializeField] UnityEditor.SceneAsset sceneAsset;
#endif
    [SerializeField, HideInInspector] string sceneName;

    public string SceneName => sceneName;

    public void OnAfterDeserialize()
    {
        
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        sceneName = sceneAsset != null ? sceneAsset.name : "";
#endif
    }
}