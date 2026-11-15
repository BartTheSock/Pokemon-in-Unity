using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPokemonGiver : MonoBehaviour, ISavable
{
    [SerializeField] Pokemon pokemonToGive1;
    [SerializeField] Pokemon pokemonToGive2;
    [SerializeField] Pokemon pokemonToGive3;
    [SerializeField] Dialog dialog;

    bool given = false;

    public IEnumerator GivePokemon(PlayerController player)
    {
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialog(dialog,
            new List<string>() { pokemonToGive1.Base.PokemonName, pokemonToGive2.Base.PokemonName, pokemonToGive3.Base.PokemonName },
            (choiceIndex) => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            pokemonToGive1.Initialize(false);
            player.GetComponent<PokemonParty>().AddPokemon(pokemonToGive1);

            given = true;

            AudioManager.i.PlaySfx(AudioID.PokemonObtained, pauseMusic: true);

            yield return DialogManager.Instance.ShowDialogText($"playerName received {pokemonToGive1.Base.PokemonName}!");
        }
        else if (selectedChoice == 1)
        {
            pokemonToGive2.Initialize(false);
            player.GetComponent<PokemonParty>().AddPokemon(pokemonToGive2);

            given = true;

            AudioManager.i.PlaySfx(AudioID.PokemonObtained, pauseMusic: true);

            yield return DialogManager.Instance.ShowDialogText($"playerName received {pokemonToGive2.Base.PokemonName}!");
        }
        else if (selectedChoice == 2)
        {
            pokemonToGive3.Initialize(false);
            player.GetComponent<PokemonParty>().AddPokemon(pokemonToGive3);

            given = true;

            AudioManager.i.PlaySfx(AudioID.PokemonObtained, pauseMusic: true);

            yield return DialogManager.Instance.ShowDialogText($"playerName received {pokemonToGive3.Base.PokemonName}!");
        }
    }

    public bool CanBeGiven()
    {
        return (pokemonToGive1 != null && pokemonToGive2 != null && pokemonToGive3 != null && !given);
    }

    public object CaptureState()
    {
        var saveData = new FirstPokemonGiverSaveData()
        {
            gaveFirstPokemon = given,
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (FirstPokemonGiverSaveData)state;
        given = saveData.gaveFirstPokemon;
    }
}

[Serializable]
public class FirstPokemonGiverSaveData
{
    public bool gaveFirstPokemon;
}