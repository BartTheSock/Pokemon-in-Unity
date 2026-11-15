using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTSUtilities.StateMachine;

public class StorageState : State<GameController>
{
    [SerializeField] PokemonStorageUI storageUI;

    bool isMovingPokemon = false;
    int selectedSlotToMove = 0;
    Pokemon selectedPokemonToMove = null;

    PokemonParty playerParty;

    public static StorageState i { get; private set; }

    private void Awake()
    {
        i = this;
        playerParty = PokemonParty.GetPlayerParty();
    }

    GameController gameController;

    public override void Enter(GameController owner)
    {
        gameController = owner;

        storageUI.gameObject.SetActive(true);
        storageUI.SetDataInPartySlots();
        storageUI.SetDataInStorageSlots();

        storageUI.OnSelected += OnSlotSelected;
        storageUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        storageUI.HandleUpdate();
    }

    public override void Exit()
    {
        storageUI.gameObject.SetActive(false);
        AudioManager.i.PlaySfx(AudioID.PCOff, pauseMusic: false);
        storageUI.OnSelected -= OnSlotSelected;
        storageUI.OnBack -= OnBack;
    }

    void OnSlotSelected(int slotIndex)
    {
        if (!isMovingPokemon)
        {
            var pokemon = storageUI.TakePokemonFromSlot(slotIndex);
            if (pokemon != null)
            {
                isMovingPokemon = true;
                selectedSlotToMove = slotIndex;
                selectedPokemonToMove = pokemon;
            }
        }
        else
        {
            isMovingPokemon = false;

            int firstSlotIndex = selectedSlotToMove;
            int secondSlotIndex = slotIndex;

            var secondPokemon = storageUI.TakePokemonFromSlot(slotIndex);

            if (secondPokemon == null && storageUI.IsPartySlot(firstSlotIndex) && storageUI.IsPartySlot(secondSlotIndex))
            {
                storageUI.PutPokemonIntoSlot(selectedPokemonToMove, firstSlotIndex);
                storageUI.SetDataInStorageSlots();
                storageUI.SetDataInPartySlots();
                return;
            }

            storageUI.PutPokemonIntoSlot(selectedPokemonToMove, secondSlotIndex);
            
            if (secondPokemon != null)
                storageUI.PutPokemonIntoSlot(secondPokemon, firstSlotIndex);

            playerParty.PokemonList.RemoveAll(p => p == null);
            playerParty.PartyUpdated();

            storageUI.SetDataInStorageSlots();
            storageUI.SetDataInPartySlots();
        }
    }

    void OnBack()
    {
        if (isMovingPokemon)
        {
            isMovingPokemon = false;
            storageUI.PutPokemonIntoSlot(selectedPokemonToMove, selectedSlotToMove);
            storageUI.SetDataInStorageSlots();
            storageUI.SetDataInPartySlots();
        }
        else
            gameController.StateMachine.Pop();
    }
}