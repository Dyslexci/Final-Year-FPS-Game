using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controller class but for the hub-world - due to several calls on player classes to the game controller, this needs to handle those calls.
/// </summary>
public class HubWorldController : MonoBehaviour
{
    public static HubWorldController instance;
    public OptionsManager[] optionsManagers;
    public bool paused;
    public AudioSource deathEntranceTeleportAudio;
    VolumeProfile postProc;
    ColorAdjustments colours;

    private void Awake()
    {
        instance = this;
        if (PlayerPrefs.HasKey("version"))
        {
            if (!PlayerPrefs.GetString("version").Equals(Application.version))
            {
                Debug.LogWarning("New version: PlayerPrefs deleted");
                PlayerPrefs.DeleteAll();
            }
        } else
        {
            Debug.LogWarning("New version: PlayerPrefs deleted");
            PlayerPrefs.DeleteAll();
        }
        PlayerPrefs.SetString("version", Application.version);
        if (PlayerPrefs.HasKey("GameStarted"))
        {
            CharacterStatisticsController.InitialiseFromPlayerPrefs();
        } else
        {
            PlayerPrefs.SetInt("GameStarted", 1);
            CharacterStatisticsController.InitialiseBaseStatistics(false);
            CharacterStatisticsController.SaveGameState();
            PlayerPrefs.Save();
        }
        if(!PlayerPrefs.HasKey("DashDamageUpgrade"))
        {
            PlayerPrefs.SetInt("DashDamageUpgrade", 0);
        }
        postProc = GameObject.Find("Post Processing Volume").GetComponent<Volume>().profile;
        postProc.TryGet(out colours);
    }

    private void Start()
    {
        optionsManagers[0].InitialiseSavedOptions();
    }

    public void StartGame()
    {
        optionsManagers[1].InitialiseSavedOptions();
    }

    public void ManageDeathSpawnIn()
    {
        optionsManagers[1].InitialiseSavedOptions();
        StartCoroutine(DeathFade());
    }

    IEnumerator DeathFade()
    {
        deathEntranceTeleportAudio.Play();
        float exposure = 25;
        colours.postExposure.Override(exposure);
        while (exposure > 0)
        {
            exposure -= 25f * Time.deltaTime;
            colours.postExposure.Override(exposure);
            yield return new WaitForEndOfFrame();
        }
    }

    public void ResetGameStats()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void Pause()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        PauseMenuController.instance.DisplayPauseMenu();
    }

    public void PauseWithoutUI()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Unpause()
    {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        PauseMenuController.instance.HidePauseMenu(true);
    }
}
