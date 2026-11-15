using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableItem
{
    void Initialize();
    void Clear();
    void OnSelectionChanged(bool selected);
}