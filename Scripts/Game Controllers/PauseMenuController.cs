using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides methods for the pause menu buttons to access.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController instance;
    public EventSystem eventSys;
    public GameObject menuButton;
    public GameObject upgradeMenuButton;
    public GameObject homeShopButton;
    public GameObject pauseMenu;
    public GameObject UI;
    public GameObject optionsMenu;
    public GameObject omnipedia;
    public GameObject homeUpgradesScreen;
    CanvasGroup UICanvasGroup;

    public Animator pauseMenuAnimator;

    VolumeProfile postProc;
    DepthOfField depthOfField;

    private void Awake()
    {
        instance = this;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        UI.SetActive(true);
        if (homeUpgradesScreen)
            homeUpgradesScreen.SetActive(false);
        postProc = GameObject.Find("Post Processing Volume").GetComponent<Volume>().profile;
        postProc.TryGet(out depthOfField);
        depthOfField.active = false;
        UICanvasGroup = UI.GetComponent<CanvasGroup>();
        HideOmnipedia();
    }

    public void DisplayPauseMenu()
    {
        pauseMenu.SetActive(true);
        pauseMenuAnimator.SetTrigger("Play");
        UICanvasGroup.alpha = .12f;
        depthOfField.active = true;
        eventSys.SetSelectedGameObject(menuButton);
    }

    public void HidePauseMenu(bool comingFromHubController)
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        UICanvasGroup.alpha = 1;
        depthOfField.active = false;
        if(omnipedia)
            HideOmnipedia();
        if (homeUpgradesScreen)
            HideHomeUpgradesMenu();
    }

    public void DisplayOmnipedia()
    {
        if (SceneManager.GetActiveScene().name.Equals("Modules"))
        {
            omnipedia.SetActive(true);
            OmnipediaManager.instance.PopulateOmnipedia();
        }
    }

    public void HideOmnipedia()
    {
        if(SceneManager.GetActiveScene().name.Equals("Modules"))
        {
            omnipedia.SetActive(false);
        }
    }

    public void Resume()
    {
        if (SceneManager.GetActiveScene().name == "HomeArea")
            HubWorldController.instance.Unpause();
        if (SceneManager.GetActiveScene().name != "HomeArea")
            GameController.instance.Unpause();

        HidePauseMenu(false);
    }

    public void DisplayHomeUpgradesMenu()
    {
        HubWorldController.instance.PauseWithoutUI();
        homeUpgradesScreen.SetActive(true);
        eventSys.SetSelectedGameObject(homeShopButton);
    }

    public void HideHomeUpgradesMenu()
    {
        print("Hiding");
        homeUpgradesScreen.SetActive(false);
        //HubWorldController.instance.Unpause();
    }

    public void QuitToMainMenu()
    {
        CharacterStatisticsController.SaveGameState();
        Application.Quit();
    }

    public void DisplayOptions()
    {
        optionsMenu.SetActive(true);
    }

    public void HideOptions()
    {
        optionsMenu.SetActive(false);
    }

    public void GiveUp()
    {
        GameController.instance.EndGame();
    }
}
