using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHUD hud;
    [SerializeField] GameObject shinySparkle;
    [SerializeField] List<Sprite> shinySparkleFrames;

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    public BattleHUD HUD
    {
        get { return hud; }
    }

    public Pokemon Pokemon { get; set; }

    Image image;
    Vector3 originalPosition;
    Vector3 frontOffsetPosition;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPosition = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;

        frontOffsetPosition = originalPosition - new Vector3(0, (Pokemon.Base.BattleOffsetY * 4f));

        if (isPlayerUnit)
        {
            if (Pokemon.IsShiny)
                image.sprite = Pokemon.Base.ShinyBackSprite;
            else
                image.sprite = Pokemon.Base.BackSprite;

            image.transform.localPosition = originalPosition;
        }
        else
        {
            if (Pokemon.IsShiny)
                image.sprite = Pokemon.Base.ShinyFrontSprite;
            else
                image.sprite = Pokemon.Base.FrontSprite;

            image.transform.localPosition = frontOffsetPosition;
        }

        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);
        pokemon.TurnCount = 0;

        transform.localScale = new Vector3(1f, 1f, 1f);
        image.color = originalColor;
        PlayEnterAnimation();
        if (Pokemon.IsShiny)
            StartCoroutine(PlayShinySparkleAnimation());
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        image.color = (selected) ? GlobalSettings.i.HighlightedColorWhiteBackground : originalColor;
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(originalPosition.x - 480f, originalPosition.y);
            image.transform.DOLocalMoveX(originalPosition.x, 0.9f);
        }
        else
        {
            image.transform.localPosition = new Vector3(frontOffsetPosition.x + 480f, frontOffsetPosition.y);
            image.transform.DOLocalMoveX(frontOffsetPosition.x, 0.9f);
        }
    }

    public void PlayPhysicalAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x + 48, 0.25f));
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x, 0.25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(frontOffsetPosition.x - 48, 0.25f));
            sequence.Append(image.transform.DOLocalMoveX(frontOffsetPosition.x, 0.25f));
        }
    }

    public void PlaySpecialAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.DOColor(Color.gray, 0.15f));
            sequence.Append(image.DOColor(originalColor, 0.15f));
            sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - 24, 0.1f));
            sequence.Append(image.transform.DOLocalMoveY(originalPosition.y, 0.1f));
            sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - 24, 0.1f));
            sequence.Append(image.transform.DOLocalMoveY(originalPosition.y, 0.1f));
        }
        else
        {
            sequence.Append(image.DOColor(Color.gray, 0.15f));
            sequence.Append(image.DOColor(originalColor, 0.15f));
            sequence.Append(image.transform.DOLocalMoveY(frontOffsetPosition.y - 18, 0.1f));
            sequence.Append(image.transform.DOLocalMoveY(frontOffsetPosition.y, 0.1f));
            sequence.Append(image.transform.DOLocalMoveY(frontOffsetPosition.y - 18, 0.1f));
            sequence.Append(image.transform.DOLocalMoveY(frontOffsetPosition.y, 0.1f));
        }
    }

    public void PlayStatusAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x - 24, 0.25f));
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x, 0.25f));
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x - 24, 0.25f));
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x, 0.25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(frontOffsetPosition.x + 24, 0.25f));
            sequence.Append(image.transform.DOLocalMoveX(frontOffsetPosition.x, 0.25f));
            sequence.Append(image.transform.DOLocalMoveX(frontOffsetPosition.x + 24, 0.25f));
            sequence.Append(image.transform.DOLocalMoveX(frontOffsetPosition.x, 0.25f));
        }
    }

    public void PlayPhysicalHitAnimation()
    {
        var sequence = DOTween.Sequence();
        for (int i = 0; i < 4; i++)
        {
            sequence.Append(image.DOFade(1f, 0f));
            sequence.Append(image.DOFade(1f, 0.1f));
            sequence.Append(image.DOFade(0f, 0f));
            sequence.Append(image.DOFade(0f, 0.1f));
        }
        sequence.Append(image.DOFade(1f, 0f));
        sequence.Append(image.DOFade(1f, 0.1f));
    }

    public void PlaySpecialHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.2f));
        sequence.Append(image.DOColor(originalColor, 0.2f));
        sequence.Append(image.DOColor(Color.gray, 0.2f));
        sequence.Append(image.DOColor(originalColor, 0.2f));
    }

    public void PlayStatusHitAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            for (int i = 0; i < 3; i++)
            {
                sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - 24, 0.15f));
                sequence.Append(image.transform.DOLocalMoveY(originalPosition.y, 0.15f));
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                sequence.Append(image.transform.DOLocalMoveY(frontOffsetPosition.y - 18, 0.15f));
                sequence.Append(image.transform.DOLocalMoveY(frontOffsetPosition.y, 0.15f));
            }
        }
    }

    public void PlayExitAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = originalPosition;
            image.transform.DOLocalMoveX(originalPosition.x - 480, 0.9f);
        }
        else
        {
            image.transform.localPosition = frontOffsetPosition;
            image.transform.DOLocalMoveX(frontOffsetPosition.x + 480, 0.9f);
        }
    }

    public void PlayFaintedAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - 120, 0.5f));
            sequence.Join(image.DOFade(0f, 0.5f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveY(frontOffsetPosition.y - 120, 0.5f));
            sequence.Join(image.DOFade(0f, 0.5f));
        }
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0f, 0.5f));
        sequence.Join(image.transform.DOLocalMoveY(originalPosition.y + 64f, 0.5f));
        sequence.Join(image.transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.5f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        image.transform.localPosition = originalPosition + new Vector3(0f, 32f);

        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1f, 0.5f));
        sequence.Join(image.transform.DOLocalMoveY(frontOffsetPosition.y, 0.5f));
        sequence.Join(image.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayShinySparkleAnimation()
    {
        yield return new WaitForSeconds(1f);

        var shinySparkleObject = Instantiate(shinySparkle, transform.position, Quaternion.identity);
        var sparkle = shinySparkleObject.GetComponent<SpriteRenderer>();
        
        if (!isPlayerUnit)
            AudioManager.i.PlaySfx(AudioID.ShinySparkle, pauseMusic: false);

        for (int i = 0; i < shinySparkleFrames.Count; i++)
        {
            sparkle.sprite = shinySparkleFrames[i];
            yield return new WaitForSeconds(0.02f);
        }

        Destroy(sparkle);
    }
}