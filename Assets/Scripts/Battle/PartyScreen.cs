using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BTS.GenericSelectionUI;

public class PartyScreen : SelectionUI<PokemonSlot>
{
    [SerializeField] Text messageText;

    PartyMemberUI[] partyMemberSlots;
    List<Pokemon> pokemonList;
    PokemonParty party;

    public Pokemon SelectedPartyMember => pokemonList[selectedItem];

    public void Initialize()
    {
        partyMemberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        SetSelectionSettings(SelectionType.Grid, 2);

        party = PokemonParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void SetPartyData()
    {
        pokemonList = party.PokemonList;

        ClearItems();

        for (int i = 0; i < partyMemberSlots.Length; i++)
        {
            if (i < pokemonList.Count)
            {
                partyMemberSlots[i].gameObject.SetActive(true);
                partyMemberSlots[i].Initialize(pokemonList[i]);
            }
            else
                partyMemberSlots[i].gameObject.SetActive(false);
        }

        var pokemonSlots = partyMemberSlots.Select(m => m.GetComponent<PokemonSlot>());
        SetItems(pokemonSlots.Take(pokemonList.Count).ToList());

        messageText.text = "Choose a Pokémon.";
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}