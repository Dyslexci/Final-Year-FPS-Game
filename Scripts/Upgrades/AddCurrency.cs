using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies currency related upgrades to the statistics controller - in the form of a Scriptable Object
/// </summary>
[CreateAssetMenu(menuName = "Upgrades/CurrencyBuff")]
public class AddCurrency : UpgradeEffect
{
    public int healthAdditive;
    public int shardsAdditive;
    public int creditsAdditive;
    
    public override void ApplyEffect()
    {
        CharacterStatisticsController.maxhealth += healthAdditive;
        CharacterStatisticsController.currentHealth += healthAdditive;
        UIController.instance.healthBar.maxValue = CharacterStatisticsController.maxhealth;
        UIController.instance.healthBar.currentPercent = CharacterStatisticsController.currentHealth;
        UIController.instance.maxHealthText.text = CharacterStatisticsController.maxhealth.ToString();

        CharacterStatisticsController.shards += shardsAdditive;
        CharacterStatisticsController.credits += creditsAdditive;
    }
}
