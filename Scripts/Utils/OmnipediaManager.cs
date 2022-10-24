using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the Omnipedia UI panel, and populates it with data.
/// </summary>
public class OmnipediaManager : MonoBehaviour
{
    public static OmnipediaManager instance;

    public TMP_Text attemptNumber;
    public TMP_Text roomsCleared;
    public TMP_Text currentKills;
    public TMP_Text damage;
    public TMP_Text criticalMultiplier;

    private void Awake()
    {
        instance = this;
    }

    public void PopulateOmnipedia()
    {
        attemptNumber.text = CharacterStatisticsController.attemptNumber.ToString();
        roomsCleared.text = CharacterStatisticsController.currentRoom.ToString();
        currentKills.text = CharacterStatisticsController.kills.ToString();
        damage.text = CharacterStatisticsController.primaryMaxDamage.ToString();
        criticalMultiplier.text = CharacterStatisticsController.critMultiplier.ToString();
    }
}
