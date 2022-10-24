using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies attack related upgrades to the statistics controller. Inherits from the UpgradeEffect scriptable object parent class.
/// </summary>
[CreateAssetMenu(menuName = "Upgrades/AttackBuff")]
public class AttackStatsUpgrade : UpgradeEffect
{
    public float critChance;
    public float critMultiplier;

    public float primaryDamage;
    public float critDuration;

    public float secondaryDamage;
    public int secondaryNumberOfShots;
    public float secondaryAttackAngle;

    public override void ApplyEffect()
    {
        CharacterStatisticsController.critChance += critChance;
        CharacterStatisticsController.critMultiplier += critMultiplier;
        CharacterStatisticsController.primaryMaxDamage += primaryDamage;
        CharacterStatisticsController.primaryMinDamage += primaryDamage;
        CharacterStatisticsController.primaryCritDamage += primaryDamage;
        CharacterStatisticsController.critDuration += critDuration;
        CharacterStatisticsController.secondaryDamage += secondaryDamage;
        CharacterStatisticsController.secondaryNumberOfShots += secondaryNumberOfShots;
        CharacterStatisticsController.secondaryAttackAngle += secondaryAttackAngle;
    }
}
