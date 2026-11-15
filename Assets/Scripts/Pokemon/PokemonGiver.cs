using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonGiver : MonoBehaviour, ISavable
{
    [SerializeField] Pokemon pokemonToGive;
    [SerializeField] Dialog dialog;

    bool given = false;

    public IEnumerator GivePokemon(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        pokemonToGive.Initialize(false);
        player.GetComponent<PokemonParty>().AddPokemon(pokemonToGive);

        given = true;

        AudioManager.i.PlaySfx(AudioID.PokemonObtained, pauseMusic: true);

        yield return DialogManager.Instance.ShowDialogText($"playerName received {pokemonToGive.Base.PokemonName}!");
    }

    public bool CanBeGiven()
    {
        return (pokemonToGive != null && !given);
    }

    public object CaptureState()
    {
        var saveData = new PokemonGiverSaveData()
        {
            gavePokemon = given,
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        given = (bool)state;
    }
}

[Serializable]
public class PokemonGiverSaveData
{
    public bool gavePokemon;
}