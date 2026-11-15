using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayTimeTimerUI : MonoBehaviour
{
    [SerializeField] Text timeText;
    private string timeString;
    private string daysString;
    private string hoursString;
    private string minutesString;
    private string secondsString;

    private void Start()
    {
        PlayTimeTimer.i.OnTimeIncremented += SetTimeText;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetTimeText();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void SetTimeText()
    {
        if (PlayTimeTimer.i.Seconds < 10)
            secondsString = $" : 0{PlayTimeTimer.i.Seconds}";
        else
            secondsString = $" : {PlayTimeTimer.i.Seconds}";

        if (PlayTimeTimer.i.Minutes < 10)
            minutesString = $" : 0{PlayTimeTimer.i.Minutes}";
        else
            minutesString = $" : {PlayTimeTimer.i.Minutes}";

        if (PlayTimeTimer.i.Days > 0)
        {
            if (PlayTimeTimer.i.Hours < 10)
                hoursString = $" : 0{PlayTimeTimer.i.Hours}";
            else
                hoursString = $" : {PlayTimeTimer.i.Hours}";
        }
        else
            hoursString = $"{PlayTimeTimer.i.Hours}";
        
        if (PlayTimeTimer.i.Days > 0)
            daysString = $"{PlayTimeTimer.i.Days}";

        timeString = $"{daysString}{hoursString}{minutesString}{secondsString}";
        timeText.text = $"{timeString}";
    }
}