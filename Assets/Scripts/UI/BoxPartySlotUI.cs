using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxPartySlotUI : MonoBehaviour
{
    [SerializeField] Text pokemonNameText;
    [SerializeField] Text pokemonLevelText;
    [SerializeField] Image pokemonIcon;
    [SerializeField] Image shinySymbol;

    public void SetData(Pokemon pokemon)
    {
        pokemonNameText.text = pokemon.Base.PokemonName;
        pokemonLevelText.text = $"Lv. {pokemon.Level}";
        pokemonIcon.sprite = pokemon.Base.IconSprite;
        shinySymbol.gameObject.SetActive(pokemon.IsShiny);
        pokemonIcon.color = new Color(255, 255, 255, 255);
    }

    public void ClearData()
    {
        pokemonNameText.text = "";
        pokemonLevelText.text = "";
        pokemonIcon.sprite = null;
        pokemonIcon.color = new Color(255, 255, 255, 0);
        shinySymbol.gameObject.SetActive(false);
    }
}