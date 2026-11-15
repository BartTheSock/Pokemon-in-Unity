using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour, ISavable
{
    [SerializeField] Color highlightedColorWhiteBackground;
    [SerializeField] Color highlightedColorDarkBackground;
    [SerializeField] Color defaultColor;
    [SerializeField] MoveBase backupMove;
    [SerializeField] ItemBase shinyCharm;
    [SerializeField] ItemBase shinyCharmDX;

    public static GlobalSettings i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    int maxEvs = 1512;
    int maxEvPerStat = 252;
    int textSpeed = 2;
    bool avatarStyle = true;
    public Color HighlightedColorWhiteBackground => highlightedColorWhiteBackground;
    public Color HighlightedColorDarkBackground => highlightedColorDarkBackground;
    public Color DefaultColor => defaultColor;
    public MoveBase BackupMove => backupMove;
    public ItemBase ShinyCharm => shinyCharm;
    public ItemBase ShinyCharmDX => shinyCharmDX;
    public int MaxEvs => maxEvs;
    public int MaxEvPerStat => maxEvPerStat;
    public int TextSpeed => textSpeed;
    public bool AvatarStyle => avatarStyle;

    public void UpdateSettings(bool updateText, bool updateSkin)
    {
        if (updateText)
        {
            textSpeed = SettingsController.i.textSpeedSetting;
            DialogManager.Instance.UpdateTextSpeed();
        }

        if (updateSkin)
            avatarStyle = SettingsController.i.avatarStyleSetting;
    }

    public object CaptureState()
    {
        var saveData = new SettingsSaveData()
        {
            textSpeedSetting = textSpeed,
            avatarStyleSetting = avatarStyle,
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (SettingsSaveData)state;
        textSpeed = saveData.textSpeedSetting;
        avatarStyle = saveData.avatarStyleSetting;
        DialogManager.Instance.UpdateTextSpeed();
    }
}

[System.Serializable]
public class SettingsSaveData
{
    public int textSpeedSetting;
    public bool avatarStyleSetting;
}