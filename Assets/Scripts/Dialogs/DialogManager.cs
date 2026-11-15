using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogText;

    int lettersPerSecond;
    PlayerController player;

    public event Action OnShowDialog;
    public event Action OnDialogFinished;

    public static DialogManager Instance { get; private set; }

    private MainGameControls gameControls;
    private InputAction nextTextAction;

    private void Awake()
    {
        Instance = this;
        gameControls = new MainGameControls();
        player = GetComponent<PlayerController>();
    }
    private void Start()
    {
        UpdateTextSpeed();
    }

    private void OnEnable()
    {
        nextTextAction = gameControls.NextTextBox.NextTextBox;
        nextTextAction.Enable();
    }

    private void OnDisable()
    {
        nextTextAction.Disable();
    }

    public void UpdateTextSpeed()
    {
        if (GlobalSettings.i.TextSpeed == 0)
            lettersPerSecond = 60;
        else if (GlobalSettings.i.TextSpeed == 1)
            lettersPerSecond = 120;
        else
            lettersPerSecond = 240;
    }

    public bool IsShowing { get; set; }

    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true,
        List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        OnShowDialog?.Invoke();
        IsShowing = true;

        AudioManager.i.PlaySfx(AudioID.UISelect);

        dialogBox.SetActive(true);
        yield return TypeDialog(text);

        if (waitForInput)
            yield return new WaitUntil(() => nextTextAction.WasPressedThisFrame());

        if (choices != null && choices.Count > 1)
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);

        if (autoClose)
            CloseDialog();

        OnDialogFinished?.Invoke();
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }

    public IEnumerator ShowDialog(Dialog dialog, List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();

        IsShowing = true;
        dialogBox.SetActive(true);

        foreach (var line in dialog.Lines)
        {
            AudioManager.i.PlaySfx(AudioID.UISelect);
            yield return TypeDialog(line);
            yield return new WaitUntil(() => nextTextAction.WasPressedThisFrame());
        }

        if (choices != null && choices.Count > 1)
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);

        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogFinished?.Invoke();
    }

    public void HandleUpdate()
    {
        
    }

    public IEnumerator TypeDialog(string dialog)
    {
        var player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Exclude);
        string playerName = player.PlayerName;
        string replacedDialog = dialog.Replace("playerName", playerName);
        dialogText.text = "";
        for (int i = 0; i < replacedDialog.ToCharArray().Length; i++)
        {
            if (GlobalSettings.i.TextSpeed == 2)
            {
                dialogText.text += replacedDialog.ToCharArray()[i];
                i++;
                if (i < replacedDialog.ToCharArray().Length)
                    dialogText.text += replacedDialog.ToCharArray()[i];
                yield return new WaitForEndOfFrame();
            }
            else
            {
                dialogText.text += replacedDialog.ToCharArray()[i];
                yield return new WaitForSeconds(1f / (float)lettersPerSecond);
            }
        }
    }
}