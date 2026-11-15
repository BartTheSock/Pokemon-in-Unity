using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] ChoiceText choiceTextPrefab;

    bool choiceSelected = false;

    List<ChoiceText> choiceTexts;
    int currentChoice;

    private MainGameControls gameControls;
    private InputAction chooseAction;
    private InputAction confirmAction;

    private void Awake()
    {
        gameControls = new MainGameControls();
    }

    private void OnEnable()
    {
        chooseAction = gameControls.CountSelector.Select;
        confirmAction = gameControls.CountSelector.Confirm;
        chooseAction.Enable();
        confirmAction.Enable();
    }

    private void OnDisable()
    {
        chooseAction.Disable();
        confirmAction.Disable();
    }
    public IEnumerator ShowChoices(List<string> choices, Action<int> onChoiceSelected)
    {
        choiceSelected = false;
        currentChoice = 0;
        gameObject.SetActive(true);

        // Delete existing choices
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        choiceTexts = new List<ChoiceText>();
        foreach (var choice in choices)
        {
            var choiceTextObject = Instantiate(choiceTextPrefab, transform);
            choiceTextObject.TextField.text = choice;
            choiceTexts.Add(choiceTextObject);
        }

        yield return new WaitUntil(() => choiceSelected == true);

        onChoiceSelected?.Invoke(currentChoice);
        gameObject.SetActive(false);
    }

    public void Update()
    {
        float vertical = chooseAction.ReadValue<float>();

        if (vertical == 1f && chooseAction.WasPressedThisFrame())
            currentChoice--;
        else if (vertical == -1f && chooseAction.WasPressedThisFrame())
            currentChoice++;
        
        currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);

        for (int i = 0; i < choiceTexts.Count; i++)
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }

        if (confirmAction.WasPressedThisFrame())
            choiceSelected = true;
    }
}