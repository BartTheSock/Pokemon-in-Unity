using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CountSelectorUI : MonoBehaviour
{
    [SerializeField] Text countText;
    [SerializeField] Text priceText;

    bool selected;
    int currentCount;

    int maxCount;
    int pricePerUnit;

    float selectTimer = 0;
    const float selectSpeed = 5;

    private MainGameControls gameControls;
    private InputAction countSelector;
    private InputAction confirmAction;
    private void Awake()
    {
        gameControls = new MainGameControls();
    }
    private void OnEnable()
    {
        countSelector = gameControls.CountSelector.Select;
        confirmAction = gameControls.CountSelector.Confirm;
        countSelector.Enable();
        confirmAction.Enable();
    }

    private void OnDisable()
    {
        countSelector.Disable();
        confirmAction.Disable();
    }

    public IEnumerator ShowSelector(int maxCount, int pricePerUnit,
        Action<int> onCountSelected)
    {
        this.maxCount = maxCount;
        this.pricePerUnit = pricePerUnit;

        selected = false;
        currentCount = 1;

        gameObject.SetActive(true);
        SetValues();

        yield return new WaitUntil(() => selected == true);

        onCountSelected?.Invoke(currentCount);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (selectTimer > 0)
            selectTimer = Mathf.Clamp(selectTimer - Time.deltaTime, 0, selectTimer);

        int previousCount = currentCount;

        float select = countSelector.ReadValue<float>();

        if (selectTimer == 0 && Mathf.Abs(select) == 1f)
        {
            if (select == 1f)
                currentCount++;
            else if (select == -1f)
                currentCount--;
            selectTimer = 1 / selectSpeed;
        }

        if (currentCount > maxCount)
            currentCount = 1;
        else if (currentCount < 1)
            currentCount = maxCount;

        if (currentCount != previousCount)
            SetValues();

        if (confirmAction.WasPressedThisFrame())
            selected = true;
    }

    void SetValues()
    {
        countText.text = "x " + currentCount;
        priceText.text = "$ " + (pricePerUnit * currentCount);
    }
}