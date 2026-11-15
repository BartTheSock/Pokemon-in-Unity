using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using BTS.GenericSelectionUI;

public class PokemonStorageUI : SelectionUI<StorageImageSlot>
{
    [SerializeField] List<StorageImageSlot> boxSlots;
    [SerializeField] Image movingPokemonIcon;
    [SerializeField] Text boxNameText;

    List<BoxPartySlotUI> partySlots = new List<BoxPartySlotUI>();
    List<BoxStorageSlotUI> storageSlots = new List<BoxStorageSlotUI>();

    List<Image> boxSlotImages;

    PokemonParty pokemonParty;
    PokemonStorageBoxes storageBoxes;

    int totalColumns = 7;

    public int SelectedBox { get; private set; } = 0;

    private MainGameControls storageControls;
    private InputAction boxSelector;
    private InputAction selectBottomBoxSlotFromParty;
    protected override void Awake()
    {
        storageControls = new MainGameControls();
        foreach (var boxSlot in boxSlots)
        {
            var storageSlot = boxSlot.GetComponent<BoxStorageSlotUI>();
            if (storageSlot != null)
                storageSlots.Add(storageSlot);
            else
                partySlots.Add(boxSlot.GetComponent<BoxPartySlotUI>());
        }

        pokemonParty = PokemonParty.GetPlayerParty();
        storageBoxes = PokemonStorageBoxes.GetPlayerStorageBoxes();
        boxSlotImages = boxSlots.Select(b => b.transform.GetChild(0).GetComponent<Image>()).ToList();
        movingPokemonIcon.gameObject.SetActive(false);
        base.Awake();
    }
    protected override void OnEnable()
    {
        boxSelector = storageControls.PokemonStorage.SelectBox;
        selectBottomBoxSlotFromParty = storageControls.PokemonStorage.SelectLastBoxSlotFromParty;
        boxSelector.Enable();
        selectBottomBoxSlotFromParty.Enable();
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        boxSelector.Disable();
        selectBottomBoxSlotFromParty.Disable();
        base.OnDisable();
    }

    private void Start()
    {
        SetItems(boxSlots);
        SetSelectionSettings(SelectionType.Grid, totalColumns);
    }

    public void SetDataInPartySlots()
    {
        for (int i = 0; i < partySlots.Count; i++)
        {
            if (i < pokemonParty.PokemonList.Count)
                partySlots[i].SetData(pokemonParty.PokemonList[i]);
            else
                partySlots[i].ClearData();
        }
    }

    public void SetDataInStorageSlots()
    {
        for (int i = 0; i < storageSlots.Count; i++)
        {
            var pokemon = storageBoxes.GetPokemon(SelectedBox, i);
            if (pokemon != null)
                storageSlots[i].SetData(pokemon);
            else
                storageSlots[i].ClearData();
        }
    }

    public override void HandleUpdate()
    {
        int previousSelectedBox = SelectedBox;

        float select = boxSelector.ReadValue<float>();

        if (select == -1f && boxSelector.WasPressedThisFrame())
            SelectedBox = SelectedBox > 0 ? SelectedBox - 1 : storageBoxes.NumberOfBoxes - 1;
        else if (select == 1f && boxSelector.WasPressedThisFrame())
            SelectedBox = (SelectedBox + 1) % storageBoxes.NumberOfBoxes;

        if (previousSelectedBox != SelectedBox)
        {
            SetDataInStorageSlots();
            UpdateSelectionInUI();
        }

        if (selectedItem == totalColumns * 5 && selectBottomBoxSlotFromParty.WasPressedThisFrame())
            selectedItem = (totalColumns * 4);

        base.HandleUpdate();
    }

    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();

        boxNameText.text = $"Box {SelectedBox + 1}";

        if (movingPokemonIcon.gameObject.activeSelf)
            movingPokemonIcon.transform.position = boxSlotImages[selectedItem].transform.position + (Vector3.up * 50f);
    }

    public bool IsPartySlot(int slotIndex)
    {
        return slotIndex % totalColumns == 0;
    }

    public Pokemon TakePokemonFromSlot(int slotIndex)
    {
        Pokemon pokemon;
        if (IsPartySlot(slotIndex))
        {
            int partyIndex = slotIndex / totalColumns;
            
            if (partyIndex >= pokemonParty.PokemonList.Count)
                return null;

            pokemon = pokemonParty.PokemonList[partyIndex];
            if (pokemon == null)
                return null;

            pokemonParty.PokemonList[partyIndex] = null;
        }
        else
        {
            int boxSlotIndex = slotIndex - (slotIndex / totalColumns + 1);
            pokemon = storageBoxes.GetPokemon(SelectedBox, boxSlotIndex);
            if (pokemon == null)
                return null;

            storageBoxes.RemovePokemon(pokemon, SelectedBox, boxSlotIndex);
        }

        movingPokemonIcon.sprite = boxSlotImages[slotIndex].sprite;
        movingPokemonIcon.transform.position = boxSlotImages[slotIndex].transform.position + (Vector3.up * 50f);
        boxSlotImages[slotIndex].color = new Color(1, 1, 1, 0);
        movingPokemonIcon.gameObject.SetActive(true);

        return pokemon;
    }

    public void PutPokemonIntoSlot(Pokemon pokemon, int slotIndex)
    {
        if (IsPartySlot(slotIndex))
        {
            int partyIndex = slotIndex / totalColumns;

            if (partyIndex >= pokemonParty.PokemonList.Count)
                pokemonParty.PokemonList.Add(pokemon);
            else
                pokemonParty.PokemonList[partyIndex] = pokemon;
        }
        else
        {
            int boxSlotIndex = slotIndex - (slotIndex / totalColumns + 1);
            storageBoxes.AddPokemon(pokemon, SelectedBox, boxSlotIndex);
        }

        movingPokemonIcon.gameObject.SetActive(false);
    }
}