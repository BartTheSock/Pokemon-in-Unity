using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutsceneAction
{
    [SerializeField] string cutsceneName;
    [SerializeField] bool waitForCompletion = true;

    public virtual IEnumerator Play()
    {
        yield break;
    }

    public string CutsceneName {
        get => cutsceneName;
        set => cutsceneName = value;
    }

    public bool WaitForCompletion => waitForCompletion;
}