using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour, ISavable
{
    [HideInInspector]
    public int money;

    public event Action OnMoneyChanged;

    public static Wallet i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        SetMoney(8000);
    }

    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke();
    }

    public void SubtractMoney(int amount)
    {
        money -= amount;
        OnMoneyChanged?.Invoke();
    }

    public void SetMoney(int amount)
    {
        money = amount;
        OnMoneyChanged?.Invoke();
    }

    public bool HasMoney(int amount)
    {
        return amount <= money;
    }

    public object CaptureState()
    {
        var saveData = new WalletSaveData()
        {
            moneyOwned = money,
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (WalletSaveData)state;
        money = saveData.moneyOwned;
    }
}

[Serializable]
public class WalletSaveData
{
    public int moneyOwned;
}