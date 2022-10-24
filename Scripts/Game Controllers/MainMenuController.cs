using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides methods for the main menu buttons to access.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject ingameCanvas;

    public GameObject mainPanel;
    public GameObject startPanel;
    public GameObject optionsPanel;
    public GameObject confirmQuitPanel;
    public GameObject mainMenuCam;
    public GameObject playerObject;
    public Transform playerCam;
    public Animator mainMenuAnimator;
    public Transform respawnPosition;

    private void Awake()
    {
        if(PlayerPrefs.HasKey("DeathEntry"))
        {
            if(PlayerPrefs.GetInt("DeathEntry") == 1)
            {
                mainMenuCam.SetActive(false);
                playerObject.SetActive(true);
                ingameCanvas.SetActive(true);
                mainMenuCanvas.SetActive(false);
                playerObject.transform.position = respawnPosition.position;
                PlayerPrefs.SetInt("DeathEntry", 0);
                HubWorldController.instance.ManageDeathSpawnIn();
                return;
            }
        }

        ResetPanels();
        mainMenuCam.SetActive(true);
        playerObject.SetActive(false);
    }

    public void ResetPanels()
    {
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public bool devOverrideINRUNStatus;

    public void StartGameButton()
    {
        //startPanel.SetActive(true);
        //mainPanel.SetActive(false);
        if(PlayerPrefs.GetInt("InRun") == 1 && !devOverrideINRUNStatus)
        {
            StartCoroutine(LoadLevel());
            return;
        }
        HubWorldController.instance.StartGame();
        mainMenuAnimator.SetTrigger("Close");
        StartCoroutine(WaitForCameraMovement());
    }

    public CanvasGroup fadeToBlack;

    IEnumerator LoadLevel()
    {
        fadeToBlack.alpha = 0;
        fadeToBlack.gameObject.SetActive(true);
        LeanTween.alphaCanvas(fadeToBlack, 1, 1);
        yield return new WaitForSeconds(1.05f);
        SceneManager.LoadScene("Modules");
    }

    IEnumerator WaitForCameraMovement()
    {
        LeanTween.move(mainMenuCam, playerCam.position, 1).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.rotate(mainMenuCam, new Vector3(0, 0, 0), 1).setEase(LeanTweenType.easeInOutQuad);
        yield return new WaitForSecondsRealtime(1);
        print("Here");
        mainMenuCam.SetActive(false);
        playerObject.SetActive(true);
        ingameCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }

    public void OptionsButton()
    {
        optionsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
