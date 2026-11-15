using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class SavingSystem : MonoBehaviour
{
    public static SavingSystem i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    static JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented,
        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
    };

    Dictionary<string, object> gameState = new Dictionary<string, object>();

    public void CaptureEntityStates(List<SavableEntity> savableEntities)
    {
        foreach (SavableEntity savable in savableEntities)
        {
            gameState[savable.UniqueId] = savable.CaptureState();
        }
    }

    public void RestoreEntityStates(List<SavableEntity> savableEntities)
    {
        foreach (SavableEntity savable in savableEntities)
        {
            string id = savable.UniqueId;
            if (gameState.ContainsKey(id))
                savable.RestoreState(gameState[id]);
        }
    }

    public void Save(string saveFile)
    {
        CaptureState(gameState);
        SaveFile(saveFile, gameState);
    }

    public void Load(string saveFile)
    {
        gameState = LoadFile(saveFile);
        RestoreState(gameState);
    }

    public void Delete(string saveFile)
    {
        File.Delete(GetPath(saveFile));
    }

    // Used to capture states of all savable objects in the game
    private void CaptureState(Dictionary<string, object> state)
    {
        foreach (SavableEntity savable in FindObjectsByType<SavableEntity>(FindObjectsSortMode.InstanceID))
        {
            state[savable.UniqueId] = savable.CaptureState();
        }
    }

    // Used to restore states of all savable objects in the game
    private void RestoreState(Dictionary<string, object> state)
    {
        foreach (SavableEntity savable in FindObjectsByType<SavableEntity>(FindObjectsSortMode.InstanceID))
        {
            string id = savable.UniqueId;
            if (state.ContainsKey(id))
                savable.RestoreState(state[id]);
        }
    }

    public void RestoreEntity(SavableEntity entity)
    {
        if (gameState.ContainsKey(entity.UniqueId))
            entity.RestoreState(gameState[entity.UniqueId]);
    }

    void SaveFile(string saveFile, Dictionary<string, object> state)
    {
        string path = GetPath(saveFile);

        string json = JsonConvert.SerializeObject(state, jsonSettings);
        File.WriteAllText(path, json);
    }

    Dictionary<string, object> LoadFile(string saveFile)
    {
        string path = GetPath(saveFile);
        if (!File.Exists(path))
            return new Dictionary<string, object>();

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject <Dictionary<string, object>>(json, jsonSettings);
    }

    public bool CheckIfSaveExists(string saveFile)
    {
        return File.Exists(GetPath(saveFile));
    }

    private string GetPath(string saveFile)
    {
        return Path.Combine(Application.persistentDataPath, saveFile);
    }
}
