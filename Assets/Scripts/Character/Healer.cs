using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    [SerializeField] Dialog dialogBeforeHeal;
    [SerializeField] Dialog dialogAboutToHeal;
    [SerializeField] Dialog dialogAfterHeal;
    [SerializeField] Dialog dialogNoHeal;
    [SerializeField] Dialog dialogNoPokemon;

    public IEnumerator HealPokemon(Transform player)
    {
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialog(dialogBeforeHeal,
            new List<string>() { "Yes", "No" },
            (choiceIndex) => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            var playerParty = player.GetComponent<PokemonParty>();

            if (playerParty.PokemonList.Count > 0)
            {
                if (dialogAboutToHeal.Lines != null)
                    yield return DialogManager.Instance.ShowDialog(dialogAboutToHeal);

                yield return Fader.i.FadeIn(0.6f);
                playerParty.PokemonList.ForEach(p => p.Heal());
                playerParty.PartyUpdated();
                AudioManager.i.PlaySfx(AudioID.HealPokemon, pauseMusic: true);
                yield return new WaitForSeconds(2.5f);
                yield return Fader.i.FadeOut(0.6f);
                yield return DialogManager.Instance.ShowDialog(dialogAfterHeal);
            }
            else
                yield return DialogManager.Instance.ShowDialog(dialogNoPokemon);
        }
        else if (selectedChoice == 1)
            yield return DialogManager.Instance.ShowDialog(dialogNoHeal);
    }
}