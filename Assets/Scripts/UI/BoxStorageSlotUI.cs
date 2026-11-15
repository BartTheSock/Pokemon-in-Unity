using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxStorageSlotUI : MonoBehaviour
{
    [SerializeField] Image pokemonIcon;
    [SerializeField] Image shinySymbol;

    public void SetData(Pokemon pokemon)
    {
        pokemonIcon.sprite = pokemon.Base.IconSprite;
        shinySymbol.gameObject.SetActive(pokemon.IsShiny);
        pokemonIcon.color = new Color(255, 255, 255, 255);
    }

    public void ClearData()
    {
        pokemonIcon.sprite = null;
        shinySymbol.gameObject.SetActive(false);
        pokemonIcon.color = new Color(255, 255, 255, 0);
    }
}