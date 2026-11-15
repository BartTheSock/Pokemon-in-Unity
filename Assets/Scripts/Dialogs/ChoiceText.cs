using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceText : MonoBehaviour
{
    [SerializeField] Color highlightedColor;
    Text textField;

    private void Awake()
    {
        textField = GetComponent<Text>();
    }

    public void SetSelected(bool selected)
    {
        textField.color = (selected) ? highlightedColor : Color.black;
    }

    public Text TextField => textField;
}