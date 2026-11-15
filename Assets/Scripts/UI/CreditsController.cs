using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreditsController : MonoBehaviour
{
    [SerializeField] GameObject creditsScreen;
    [SerializeField] List<GameObject> creditsPages;

    int pageNumber = 0;

    private MainGameControls gameControls;
    private InputAction pageSelect;
    private InputAction cancelAction;
    private void Awake()
    {
        gameControls = new MainGameControls();
    }
    private void OnEnable()
    {
        pageSelect = gameControls.Credits.Select;
        cancelAction = gameControls.Credits.Cancel;
        pageSelect.Enable();
        cancelAction.Enable();
    }

    private void OnDisable()
    {
        pageSelect.Disable();
        cancelAction.Disable();
    }

    public void ResetPageNumber()
    {
        pageNumber = 0;
    }

    public void HandleUpdate()
    {
        float pageSelection = pageSelect.ReadValue<float>();

        if (pageSelection == -1f && pageSelect.WasPressedThisFrame())
        {
            if (pageNumber > 0)
                pageNumber--;
        }
        else if (pageSelection == 1f && pageSelect.WasPressedThisFrame())
        {
            if (pageNumber < creditsPages.Count - 1)
                pageNumber++;
        }

        for (int i = 0; i < creditsPages.Count; i++)
        {
            if (i == pageNumber)
                creditsPages[i].gameObject.SetActive(true);
            else
                creditsPages[i].gameObject.SetActive(false);
        }

        if (cancelAction.WasPressedThisFrame())
            creditsScreen.gameObject.SetActive(false);
    }
}