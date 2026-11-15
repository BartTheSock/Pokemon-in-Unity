using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroutbeckTownFountain : MonoBehaviour
{
    [SerializeField] List<Sprite> troutbeckTownFountainSprites;

    private void Start()
    {
        StartCoroutine(HandleUpdate());
    }

    public IEnumerator HandleUpdate()
    {
        var fountainAnimation = GetComponent<SpriteRenderer>();
        while (true)
        {
            fountainAnimation.sprite = troutbeckTownFountainSprites[0];
            yield return new WaitForSeconds(0.25f);
            fountainAnimation.sprite = troutbeckTownFountainSprites[1];
            yield return new WaitForSeconds(0.25f);
            fountainAnimation.sprite = troutbeckTownFountainSprites[2];
            yield return new WaitForSeconds(0.25f);
        }
    }
}