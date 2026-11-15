using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using E7.Introloop;
using BTS.GenericSelectionUI;

public class MainMenuController : SelectionUI<TextSlot>
{
    [SerializeField] Image titleScreen;
    [SerializeField] IntroloopAudio titleScreenMusic;
    [SerializeField] GameObject mainMenu;
    [SerializeField] CreditsController creditsController;

    [SerializeField] float startGameDelay;
    private float startGameTimeStamp = 0f;

    private void Start()
    {
        startGameTimeStamp = Time.realtimeSinceStartup + startGameDelay + 1f;
        titleScreen.color = Color.black;

        var sequence = DOTween.Sequence();
        sequence.Append(titleScreen.DOColor(Color.white, startGameDelay));

        StartCoroutine(StartMusic());

        var textSlots = GetComponentsInChildren<TextSlot>().ToList();

        if (SavingSystem.i.CheckIfSaveExists("saveSlot"))
            SetItems(textSlots);
        else
        {
            SetItems(textSlots.TakeLast(2).ToList());
            textSlots.First().GetComponent<Text>().color = new Color(0.75f, 0.75f, 0.75f, 1f);
        }

        OnSelected += OnItemSelected;
    }

    public IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(0.05f);
        AudioManager.i.PlayMusic(titleScreenMusic, true);
    }

    private void Update()
    {
        if (Time.realtimeSinceStartup > startGameTimeStamp)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            HandleUpdate();

            if (creditsController.gameObject.activeSelf)
                creditsController.HandleUpdate();
        }
        else
            transform.localScale = new Vector3(0f, 0f, 0f);
    }

    void OnItemSelected(int selection)
    {
        if (!creditsController.gameObject.activeSelf)
        {
            if (SavingSystem.i.CheckIfSaveExists("saveSlot"))
            {
                if (selection == 0) // Continue
                {
                    DontDestroyOnLoad(gameObject);
                    SceneManager.LoadScene(1);
                    SavingSystem.i.Load("saveSlot");
                    Destroy(gameObject);
                }
                else if (selection == 1) // New Game
                {
                    SavingSystem.i.Delete("saveSlot");
                    SceneManager.LoadScene(1);
                }
                else if (selection == 2) // Credits
                {
                    creditsController.ResetPageNumber();
                    creditsController.gameObject.SetActive(true);
                }
            }
            else
            {
                if (selection == 0) // New Game
                {
                    SavingSystem.i.Delete("saveSlot");
                    SceneManager.LoadScene(1);
                }
                else if (selection == 1) // Credits
                {
                    creditsController.ResetPageNumber();
                    creditsController.gameObject.SetActive(true);
                }
            }
        }
    }
}