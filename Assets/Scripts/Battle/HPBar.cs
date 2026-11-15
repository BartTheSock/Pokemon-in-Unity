using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image health;
    [SerializeField] Image healthShadow;

    [SerializeField] Text hpText;

    [SerializeField] Color highHP;
    [SerializeField] Color highHPShadow;
    [SerializeField] Color medHP;
    [SerializeField] Color medHPShadow;
    [SerializeField] Color lowHP;
    [SerializeField] Color lowHPShadow;

    public bool IsUpdating { get; private set; }

    public void SetHP(float hpNormalized, int maxHP)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);

        SetHPBarColor(hpNormalized);

        if (hpText != null)
            hpText.text = Mathf.RoundToInt(hpNormalized * (float)maxHP) + " / " + maxHP;
    }

    public IEnumerator SetHPSmooth(float newHP, int maxHP)
    {
        IsUpdating = true;
        float currentHP = health.transform.localScale.x;
        float changeAmount = currentHP - newHP;

        if (changeAmount > 0f)
        {
            while (currentHP - newHP > Mathf.Epsilon)
            {
                currentHP -= changeAmount * Time.deltaTime;
                health.transform.localScale = new Vector3(currentHP, 1f);
                
                SetHPBarColor(currentHP);
                if (hpText != null)
                    hpText.text = Mathf.RoundToInt(currentHP * (float)maxHP) + " / " + maxHP;

                yield return null;
            }
        }
        else if (changeAmount < 0f)
        {
            while (currentHP - newHP < Mathf.Epsilon)
            {
                currentHP -= changeAmount * Time.deltaTime;
                health.transform.localScale = new Vector3(currentHP, 1f);

                SetHPBarColor(currentHP);
                if (hpText != null)
                    hpText.text = Mathf.RoundToInt(currentHP * (float)maxHP) + " / " + maxHP;

                yield return null;
            }
        }
        health.transform.localScale = new Vector3(newHP, 1f);

        SetHPBarColor(newHP);
        if (hpText != null)
            hpText.text = Mathf.RoundToInt(newHP * (float)maxHP) + " / " + maxHP;

        IsUpdating = false;
    }

    public void SetHPBarColor(float normalizedHealth)
    {
        if (normalizedHealth > 0.5f)
        {
            health.color = highHP;
            healthShadow.color = highHPShadow;
        }
        else if (normalizedHealth > 0.2f)
        {
            health.color = medHP;
            healthShadow.color = medHPShadow;
        }
        else
        {
            health.color = lowHP;
            healthShadow.color = lowHPShadow;
        }
    }

    public Image Health => health;
}