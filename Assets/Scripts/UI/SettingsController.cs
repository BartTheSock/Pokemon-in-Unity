using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsController : MonoBehaviour, Interactable
{
    [HideInInspector]
    public int textSpeedSetting;
    [HideInInspector]
    public bool avatarStyleSetting;

    public static SettingsController i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public IEnumerator Interact(Transform initiator)
    {
        int selectedTextSpeed;
        int selectedAvatarStyle;

        int selectedChoiceA = 0;
        yield return DialogManager.Instance.ShowDialogText($"Would you like to change any of the settings?",
            choices: new List<string>() { "Yes", "No" },
            onChoiceSelected: (selectionA) => selectedChoiceA = selectionA);

        if (selectedChoiceA == 0)
        {
            int selectedChoiceB = 0;
            yield return DialogManager.Instance.ShowDialogText($"Which setting would you like to change?",
                choices: new List<string>() { "Text Speed", "Avatar Style", "Neither" },
                onChoiceSelected: (selectionB) => selectedChoiceB = selectionB);

            if (selectedChoiceB == 0)
            {
                selectedTextSpeed = 0;
                yield return DialogManager.Instance.ShowDialogText($"Which text speed setting do you prefer?",
                    choices: new List<string>() { "Slow", "Medium", "Fast" },
                    onChoiceSelected: (textSpeedSelection) => selectedTextSpeed = textSpeedSelection);

                textSpeedSetting = selectedTextSpeed;

                GlobalSettings.i.UpdateSettings(true, false);
            }
            else if (selectedChoiceB == 1)
            {
                selectedAvatarStyle = 0;
                yield return DialogManager.Instance.ShowDialogText($"Which avatar do you prefer?",
                    choices: new List<string>() { "Brendan", "May" },
                    onChoiceSelected: (avatarStylelection) => selectedAvatarStyle = avatarStylelection);

                if (selectedAvatarStyle == 0)
                    avatarStyleSetting = true;
                else if (selectedAvatarStyle == 1)
                    avatarStyleSetting = false;

                GlobalSettings.i.UpdateSettings(false, true);
            }
        }
    }

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }

    public bool IsMoving => false;
}