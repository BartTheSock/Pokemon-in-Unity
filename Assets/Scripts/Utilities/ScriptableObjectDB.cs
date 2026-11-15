using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDB<T> : MonoBehaviour where T : ScriptableObject
{
    static Dictionary<string, T> objectList;

    public static void Initialize()
    {
        objectList = new Dictionary<string, T>();

        var objectArray = Resources.LoadAll<T>("");
        foreach (var _object in objectArray)
        {
            if (objectList.ContainsKey(_object.name))
            {
                Debug.LogError($"There are two objects with the name {_object.name}!");
                continue;
            }

            objectList[_object.name] = _object;
        }
    }

    public static T GetObjectByName(string name)
    {
        if (!objectList.ContainsKey(name))
        {
            Debug.LogError($"There aren't any objects with the name {name}!");
            return null;
        }

        return objectList[name];
    }
}