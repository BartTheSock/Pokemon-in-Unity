using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageImageSlot : MonoBehaviour, ISelectableItem
{
    Image backgroundImage;
    [SerializeField] Color highlightedBackgroundColor;
    [SerializeField] Color highlightMovingBackgroundColor;
    [SerializeField] Color defaultBackgroundColor;

    void Awake()
    {
        backgroundImage = GetComponent<Image>();
    }

    public void Initialize()
    {
        backgroundImage.color = defaultBackgroundColor;
    }

    public void Clear()
    {
        backgroundImage.color = defaultBackgroundColor;
    }

    public void OnSelectionChanged(bool selected)
    {
        backgroundImage.color = (selected) ? highlightedBackgroundColor : defaultBackgroundColor;
    }
}