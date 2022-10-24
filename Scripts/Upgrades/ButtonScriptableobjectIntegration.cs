using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Provides an interface between the scriptable object and the button - displaying the title, description, cost and etc. visually on screen
/// </summary>
public class ButtonScriptableobjectIntegration : MonoBehaviour
{
    public UpgradeEffect upgradeObj;
    public TMP_Text title;
    public TMP_Text desc;
    public TMP_Text cost;
    public TMP_Text multiplier;
    public Button button;
    public HomeUpgradesDispensorButton upgradeController;

    private void Awake()
    {
        title.text = upgradeObj.title;
        desc.text = upgradeObj.description;
        cost.text = upgradeObj.cost.ToString();
        if (!PlayerPrefs.HasKey(upgradeObj.title))
            PlayerPrefs.SetInt(upgradeObj.title, 0);
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("shards"))
        {
            button.enabled = false;
            cost.color = Color.red;
            return;
        }
        if (CharacterStatisticsController.shards < upgradeObj.cost)
        {
            button.enabled = false;
            cost.color = Color.red;
        }
    }

    /// <summary>
    /// Updates info - prices and multipliers change as the player buys upgrades.
    /// </summary>
    public void UpdateInfo()
    {
        if(PlayerPrefs.HasKey(upgradeObj.title))
        {
            multiplier.text = PlayerPrefs.GetInt(upgradeObj.title).ToString() + "x";
            cost.text = (upgradeObj.cost * (1 + (PlayerPrefs.GetInt(upgradeObj.title) * .2f))).ToString();
        } else
        {
            multiplier.text = "0x";
            cost.text = upgradeObj.cost.ToString();
        }
        if (!PlayerPrefs.HasKey("shards"))
        {
            button.enabled = false;
            cost.color = Color.red;
            return;
        }
        if (CharacterStatisticsController.shards < (upgradeObj.cost * (1 + (PlayerPrefs.GetInt(upgradeObj.title) * .2f))))
        {
            button.enabled = false;
            cost.color = Color.red;
        }
        if(upgradeObj.title == "Leg-Spring Actuators" && PlayerPrefs.HasKey("hasDoubleJump")) {
            if(PlayerPrefs.GetInt("hasDoubleJump") == 1)
            {
                button.enabled = false;
                cost.color = Color.red;
            }
        }
    }

    /// <summary>
    /// The upgrade button calls this method.
    /// </summary>
    public void Select()
    {
        if(CharacterStatisticsController.shards >= (upgradeObj.cost * (1 + (PlayerPrefs.GetInt(upgradeObj.title) * .2f))))
        {
            CharacterStatisticsController.shards -= (int)(upgradeObj.cost * (1 + (PlayerPrefs.GetInt(upgradeObj.title) * .2f)));
            upgradeObj.ApplyEffect();
            if(PlayerPrefs.HasKey(upgradeObj.title))
            {
                PlayerPrefs.SetInt(upgradeObj.title, PlayerPrefs.GetInt(upgradeObj.title) + 1);
            } else
            {
                PlayerPrefs.SetInt(upgradeObj.title, 1);
            }
            CharacterStatisticsController.SaveGameState();
        }
        UpdateInfo();
        upgradeController.UpdateButtonInfos();
    }
}
