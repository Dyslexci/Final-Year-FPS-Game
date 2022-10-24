using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject UICanvas;

    public ProgressBar healthBar;
    public TMP_Text maxHealthText;
    public CanvasGroup blackFaderPanel;
    PlayerInputs inputs;

    public TMP_Text creditsText;
    public TMP_Text shardsText;

    public GameObject upgradeUIPanel;
    public TMP_Text firstUpgradeTitle;
    public TMP_Text firstUpgradeDesc;
    public TMP_Text secondUpgradeTitle;
    public TMP_Text secondUpgradeDesc;
    public TMP_Text thirdUpgradeTitle;
    public TMP_Text thirdUpgradeDesc;
    LootDropMaster activeLootDrop;

    public GameObject enemyHealthBarContainer;
    public ProgressBar enemyHealthBar;
    public TMP_Text enemyMaxHealth;
    public TMP_Text enemyName;

    private void Awake()
    {
        inputs = GetComponent<PlayerInputs>();
        instance = this;
        blackFaderPanel.gameObject.SetActive(false);
        upgradeUIPanel.SetActive(false);
        enemyHealthBarContainer.SetActive(false);
    }

    private void Update()
    {
        Pause();
        creditsText.text = CharacterStatisticsController.credits.ToString();
        shardsText.text = CharacterStatisticsController.shards.ToString();
    }

    /// <summary>
    /// Hides the main UI canvas, removing all UI from the screen.
    /// </summary>
    public void HideAllUI()
    {
        UICanvas.SetActive(false);
    }

    /// <summary>
    /// When the game is paused, this method displays the pause menu.
    /// </summary>
    void Pause()
    {
        if (inputs.pause)
        {
            if(SceneManager.GetActiveScene().name == "Modules")
            {
                if (GameController.instance.paused)
                {
                    GameController.instance.Unpause();
                    inputs.pause = false;
                    return;
                }
                GameController.instance.Pause();
                inputs.pause = false;
            } else
            {
                if (HubWorldController.instance.paused)
                {
                    HubWorldController.instance.Unpause();
                    inputs.pause = false;
                    return;
                }
                HubWorldController.instance.Pause();
                inputs.pause = false;
            }
        }
    }

    /// <summary>
    /// Changes the UI health slider in line with the current health.
    /// </summary>
    /// <param name="newHealth"></param>
    public void UpdateHealthSlider(int newHealth)
    {
        healthBar.currentPercent = newHealth;
        maxHealthText.text = CharacterStatisticsController.maxhealth.ToString();
    }

    public void EndRoomSequence(string upgradePath)
    {
        StartCoroutine(EndRoomEnumerator(upgradePath));
    }

    IEnumerator EndRoomEnumerator(string upgradePath)
    {
        blackFaderPanel.gameObject.SetActive(true);
        while (blackFaderPanel.alpha < 1)
        {
            blackFaderPanel.alpha += .026f;
            yield return new WaitForSeconds(.05f);
        }
        GameController.instance.ChangeRoom(upgradePath);
        while (blackFaderPanel.alpha > 0)
        {
            blackFaderPanel.alpha -= .05f;
            yield return new WaitForSeconds(.05f);
        }
        blackFaderPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Populates the buttons, titles and descriptions with the randomly chosen upgrades.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="third"></param>
    /// <param name="callingLootDrop"></param>
    public void PopulateUpgradeDisplay(UpgradeEffect first, UpgradeEffect second, UpgradeEffect third, LootDropMaster callingLootDrop)
    {
        GameController.instance.PauseWithoutUI();
        upgradeUIPanel.SetActive(true);
        activeLootDrop = callingLootDrop;
        firstUpgradeTitle.text = first.title;
        firstUpgradeDesc.text = first.description;
        secondUpgradeTitle.text = second.title;
        secondUpgradeDesc.text = second.description;
        thirdUpgradeTitle.text = third.title;
        thirdUpgradeDesc.text = third.description;
    }

    /// <summary>
    /// Updates the UI healthbar for an enemy the player is looking at.
    /// </summary>
    /// <param name="_enemyCurrentHealth"></param>
    /// <param name="_enemyMaxHealth"></param>
    /// <param name="_enemyName"></param>
    public void UpdateEnemyHealthbar(int _enemyCurrentHealth, int _enemyMaxHealth, string _enemyName)
    {
        if(_enemyCurrentHealth == 0 && _enemyMaxHealth == 0 && _enemyName.Equals(""))
        {
            enemyHealthBarContainer.SetActive(false);
            return;
        }
        enemyHealthBarContainer.SetActive(true);
        enemyHealthBar.maxValue = _enemyMaxHealth;
        enemyHealthBar.currentPercent = _enemyCurrentHealth;
        enemyMaxHealth.text = _enemyMaxHealth.ToString();
        enemyName.text = _enemyName;
    }

    public void UpgradeSelection(int sel)
    {
        activeLootDrop.UpgradeSelected(sel);
        upgradeUIPanel.SetActive(false);
        GameController.instance.Unpause();
    }
}
