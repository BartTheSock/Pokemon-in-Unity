using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

namespace BTS.GenericSelectionUI
{
    public enum SelectionType { List, Grid }
    public enum SelectionCategory { Normal, Party, Storage }

    public class SelectionUI<T> : MonoBehaviour where T : ISelectableItem
    {
        List<T> items;
        protected int selectedItem = 0;

        SelectionType selectionType;
        SelectionCategory selectionCategory;
        int gridWidth = 2;

        public event Action<int> OnSelected;
        public event Action OnBack;

        public MainGameControls gameControls;
        public InputAction gridSelector;
        public InputAction listSelector;
        public InputAction confirmAction;
        public InputAction cancelAction;
        protected virtual void Awake()
        {
            gameControls = new MainGameControls();
        }

        protected virtual void OnEnable()
        {
            gridSelector = gameControls.GenericSelector.GridSelect;
            listSelector = gameControls.GenericSelector.ListSelect;
            confirmAction = gameControls.GenericSelector.Confirm;
            cancelAction = gameControls.GenericSelector.Cancel;
            gridSelector.Enable();
            listSelector.Enable();
            confirmAction.Enable();
            cancelAction.Enable();
        }

        protected virtual void OnDisable()
        {
            gridSelector.Disable();
            listSelector.Disable();
            confirmAction.Disable();
            cancelAction.Disable();
        }

        public void SetSelectionSettings(SelectionType selectionType, int gridWidth)
        {
            this.selectionType = selectionType;
            this.gridWidth = gridWidth;
        }

        public void SetItems(List<T> items)
        {
            this.items = items;
            items.ForEach(i => i.Initialize());
            UpdateSelectionInUI();
        }

        public void ClearItems()
        {
            items?.ForEach(i => i.Clear());
            this.items = null;
        }

        public virtual void HandleUpdate()
        {
            float previousSelection = selectedItem;

            if (selectionType == SelectionType.List)
                HandleListSelection();
            else if (selectionType == SelectionType.Grid)
                HandleGridSelection();

            selectedItem = Mathf.Clamp(selectedItem, 0, items.Count - 1);

            if (selectedItem != previousSelection)
                UpdateSelectionInUI();

            if (confirmAction.WasPressedThisFrame())
            {
                if (items.Count > 0)
                    OnSelected?.Invoke(selectedItem);
                else
                    OnBack?.Invoke();
            }
            else if (cancelAction.WasPressedThisFrame())
                OnBack?.Invoke();
        }

        void HandleListSelection()
        {
            var listSelect = listSelector.ReadValue<float>();

            if (Mathf.Abs(listSelect) == 1f && listSelector.WasPressedThisFrame())
            {
                if (listSelect == 1f)
                    selectedItem--;
                else if (listSelect == -1f)
                    selectedItem++;
            }
        }

        void HandleGridSelection()
        {
            var gridSelect = gridSelector.ReadValue<Vector2>();

            if ((Mathf.Abs(gridSelect.x) == 1f || Mathf.Abs(gridSelect.y) == 1f) && gridSelector.WasPressedThisFrame())
            {
                if (gridSelect.x == 1f)
                    selectedItem++;
                else if (gridSelect.x == -1f)
                    selectedItem--;
                else if (gridSelect.y == 1f)
                    selectedItem -= gridWidth;
                else if (gridSelect.y == -1f)
                    selectedItem += gridWidth;
            }
        }

        public virtual void UpdateSelectionInUI()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OnSelectionChanged(i == selectedItem);
            }
        }
    }
}