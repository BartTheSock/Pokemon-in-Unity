using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTimeTimer : MonoBehaviour, ISavable
{
    private int playTime = 0;
    private int seconds = 0;
    private int minutes = 0;
    private int hours = 0;
    private int days = 0;

    public event Action OnTimeIncremented;

    public static PlayTimeTimer i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public void ResetTimer()
    {
        playTime = 0;
        OnTimeIncremented?.Invoke();
    }

    public void StartTimer()
    {
        StartCoroutine(IncrementPlayTime());
    }

    private IEnumerator IncrementPlayTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            playTime++;
            seconds = Mathf.FloorToInt(playTime % 60);
            minutes = Mathf.FloorToInt((playTime / 60) % 60);
            hours = Mathf.FloorToInt((playTime / 3600) % 24);
            days = Mathf.FloorToInt(playTime / 86400);
            OnTimeIncremented?.Invoke();
        }
    }

    public object CaptureState()
    {
        var saveData = new PlayTimeTimerSaveData()
        {
            playTimeSeconds = playTime,
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayTimeTimerSaveData)state;
        playTime = saveData.playTimeSeconds;
        seconds = Mathf.FloorToInt(playTime % 60);
        minutes = Mathf.FloorToInt((playTime / 60) % 60);
        hours = Mathf.FloorToInt((playTime / 3600) % 24);
        days = Mathf.FloorToInt(playTime / 86400);
        OnTimeIncremented?.Invoke();
    }

    public int PlayTime => playTime;
    public int Seconds => seconds;
    public int Minutes => minutes;
    public int Hours => hours;
    public int Days => days;
}
[Serializable]
public class PlayTimeTimerSaveData
{
    public int playTimeSeconds;
}