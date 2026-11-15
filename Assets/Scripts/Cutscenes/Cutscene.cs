using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using E7.Introloop;

public class Cutscene : MonoBehaviour, IPlayerTriggerable
{
    [SerializeReference]
    [SerializeField] List<CutsceneAction> actions;
    [SerializeField] IntroloopAudio cutsceneMusic;

    public IEnumerator Play()
    {
        AudioManager.i.PlayMusic(cutsceneMusic, false);

        GameController.Instance.StateMachine.Push(CutsceneState.i);

        foreach (var action in actions)
        {
            if (action.WaitForCompletion)
                yield return action.Play();
            else
                StartCoroutine(action.Play());
        }

        GameController.Instance.StateMachine.Pop();

        AudioManager.i.PlayMusic(GameController.Instance.CurrentScene.SceneMusic, true);
    }

    public void AddAction(CutsceneAction action)
    {
#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this, "Add action to cutscene.");
#endif

        action.CutsceneName = action.GetType().ToString();
        actions.Add(action);
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Play());
    }

    public bool TriggerRepeatedly => false;
}