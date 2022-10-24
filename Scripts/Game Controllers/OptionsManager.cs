using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;

/// <summary>
/// Provides methods for the options menu to access, and loads / saves these preferences as needed. Player prefs access and saving is really annoyingly long.
/// </summary>
public class OptionsManager : MonoBehaviour
{
    public AudioMixer mixer;

    public GameObject optionsPanel;
    public GameObject optionsSideBar;
    public GameObject controlsPanel;
    public GameObject controlsSideBar;

    public SliderManager sensSlider;
    public SliderManager masterSlider;
    public SliderManager sfxSlider;
    public SliderManager musicSlider;
    public Toggle fullscreenToggle;
    public Image fullscreenToggleCheck;
    public Toggle shakeToggle;
    public Image shakeToggleCheck;
    public Toggle godModeToggle;
    public Image godModeToggleCheck;

    float masterVolume;
    float SFXVolume;
    float musicVolume;
    float sensitivity;
    string resolution;
    bool fullscreen;
    bool screenShake;
    bool godMode;

    private void Awake()
    {
        optionsPanel.SetActive(true);
        optionsSideBar.SetActive(true);
        controlsPanel.SetActive(false);
        controlsSideBar.SetActive(false);
    }

    public void InitialiseSavedOptions()
    {
        if (!PlayerPrefs.HasKey("sensitivity"))
        {
            print("Setting up new options");
            PlayerPrefs.SetFloat("sensitivity", 0.4f);
            PlayerPrefs.SetFloat("masterVolume", 1.0f);
            PlayerPrefs.SetFloat("sfxVolume", 1.0f);
            PlayerPrefs.SetFloat("musicVolume", 1.0f);
            PlayerPrefs.SetString("resolution", "1920x1080");
            PlayerPrefs.SetInt("fullscreen", 1);
            PlayerPrefs.SetInt("screenShake", 1);
            PlayerPrefs.SetInt("godMode", 0);
            sensitivity = .4f;
            masterVolume = 1;
            SFXVolume = 1;
            musicVolume = 1;
            resolution = "1920x1080";
            fullscreen = true;
            screenShake = true;
            godMode = false;
            sensSlider.mainSlider.value = .4f;
            CharacterStatisticsController.sensitivity = .4f;
            godModeToggle.isOn = false;
            godModeToggleCheck.fillAmount = 0;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            CharacterStatisticsController.screenShake = true;
            CharacterStatisticsController.godMode = false;
            print(PlayerPrefs.GetFloat("sensitivity"));
        }
        else
        {
            print("Setting up saved options");
            sensitivity = PlayerPrefs.GetFloat("sensitivity");
            masterVolume = PlayerPrefs.GetFloat("masterVolume");
            SFXVolume = PlayerPrefs.GetFloat("sfxVolume");
            musicVolume = PlayerPrefs.GetFloat("musicVolume");
            resolution = PlayerPrefs.GetString("resolution");
            fullscreen = PlayerPrefs.GetInt("fullscreen") == 1 ? true : false;
            screenShake = PlayerPrefs.GetInt("screenShake") == 1 ? true : false;
            godMode = PlayerPrefs.GetInt("godMode") == 1 ? true : false;
            sensSlider.mainSlider.value = sensitivity;
            masterSlider.mainSlider.value = masterVolume;
            sfxSlider.mainSlider.value = SFXVolume;
            musicSlider.mainSlider.value = musicVolume;
            fullscreenToggle.isOn = fullscreen;
            fullscreenToggleCheck.fillAmount = fullscreen == true ? 1 : 0;
            shakeToggle.isOn = screenShake;
            shakeToggleCheck.fillAmount = screenShake == true ? 1 : 0;
            godModeToggle.isOn = godMode;
            godModeToggleCheck.fillAmount = godMode == true ? 1 : 0;
            CharacterStatisticsController.sensitivity = sensitivity;
            mixer.SetFloat("masterVolume", Mathf.Log10(masterVolume) * 20);
            mixer.SetFloat("sfxVolume", Mathf.Log10(SFXVolume) * 20);
            mixer.SetFloat("musicVolume", Mathf.Log10(musicVolume) * 20);
            Screen.fullScreenMode = fullscreen == true ? FullScreenMode.FullScreenWindow : FullScreenMode.MaximizedWindow;
            CharacterStatisticsController.screenShake = screenShake;
            CharacterStatisticsController.godMode = godMode;
        }
    }

    public void Start()
    {
        CharacterStatisticsController.sensitivity = sensitivity;
        mixer.SetFloat("masterVolume", Mathf.Log10(masterVolume) * 20);
        mixer.SetFloat("sfxVolume", Mathf.Log10(SFXVolume) * 20);
        mixer.SetFloat("musicVolume", Mathf.Log10(musicVolume) * 20);
        Screen.fullScreenMode = fullscreen == true ? FullScreenMode.FullScreenWindow : FullScreenMode.MaximizedWindow;
        CharacterStatisticsController.screenShake = screenShake;
        CharacterStatisticsController.godMode = godMode;
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
        CharacterStatisticsController.sensitivity = sensitivity;
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("sfxVolume", SFXVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetString("resolution", resolution);
        PlayerPrefs.SetInt("fullscreen", fullscreen == true ? 1 : 0);
        CharacterStatisticsController.screenShake = screenShake;
        PlayerPrefs.SetInt("screenShake", screenShake == true ? 1 : 0);
        PlayerPrefs.SetInt("godMode", godMode == true ? 1 : 0);
        CharacterStatisticsController.godMode = godMode;
        optionsPanel.SetActive(true);
        optionsSideBar.SetActive(true);
        controlsPanel.SetActive(false);
        controlsSideBar.SetActive(false);
    }

    public void CancelOptions()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        SFXVolume = PlayerPrefs.GetFloat("sfxVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        resolution = PlayerPrefs.GetString("resolution");
        fullscreen = PlayerPrefs.GetInt("windowMode") == 1 ? true : false;
        screenShake = PlayerPrefs.GetInt("screenShake") == 1 ? true : false;
        godMode = PlayerPrefs.GetInt("godMode") == 1 ? true : false;
        optionsPanel.SetActive(true);
        optionsSideBar.SetActive(true);
        controlsPanel.SetActive(false);
        controlsSideBar.SetActive(false);
    }

    public void OpenOptionsPanel()
    {
        optionsPanel.SetActive(true);
        controlsPanel.SetActive(false);
        optionsSideBar.SetActive(true);
        controlsSideBar.SetActive(false);
    }

    public void OpenControlsPanel()
    {
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(true);
        optionsSideBar.SetActive(false);
        controlsSideBar.SetActive(true);
    }

    public void SetSensitivity(float sensitivity)
    {
        this.sensitivity = sensitivity;
    }

    public void SetVolumeMaster(float volume)
    {
        masterVolume = volume;
        mixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeSFX(float volume)
    {
        SFXVolume = volume;
        mixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeMusic(float volume)
    {
        musicVolume = volume;
        mixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetResolution(string resolution)
    {
        this.resolution = resolution;
    }

    public void SetWindowedMode(bool mode)
    {
        fullscreen = mode;
        Screen.fullScreenMode = fullscreen == true ? FullScreenMode.FullScreenWindow : FullScreenMode.MaximizedWindow;
    }

    public void ScreenShake(bool state)
    {
        screenShake = state;
    }

    public void GodMode(bool state)
    {
        godMode = state;
    }
}
