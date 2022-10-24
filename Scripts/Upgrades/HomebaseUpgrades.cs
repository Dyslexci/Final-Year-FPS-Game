using UnityEngine;

/// <summary>
/// Applies upgrades which can be purchased at the home base. These upgrades are applied directly to playerprefs instead of the statistics controller.
/// </summary>
[CreateAssetMenu(menuName = "Upgrades/HomebaseUpgrades")]
public class HomebaseUpgrades : UpgradeEffect
{
    public float secondaryMultiplierFromPrimary;
    public float primaryMultiplierFromSecondary;
    public int HPIncrease;
    public bool enableDoubleJump;

    public override void ApplyEffect()
    {
        if(PlayerPrefs.HasKey("secondaryMultiplierFromPrimary"))
        {
            PlayerPrefs.SetFloat("secondaryMultiplierFromPrimary", PlayerPrefs.GetFloat("secondaryMultiplierFromPrimary") + secondaryMultiplierFromPrimary);
        } else
        {
            PlayerPrefs.SetFloat("secondaryMultiplierFromPrimary", 1 + secondaryMultiplierFromPrimary);
        }
        if(PlayerPrefs.HasKey("primaryMultiplierFromSecondary"))
        {
            PlayerPrefs.SetFloat("primaryMultiplierFromSecondary", PlayerPrefs.GetFloat("primaryMultiplierFromSecondary") + primaryMultiplierFromSecondary);
        } else
        {
            PlayerPrefs.SetFloat("primaryMultiplierFromSecondary", 1 + primaryMultiplierFromSecondary);
        }
        PlayerPrefs.SetInt("hasDoubleJump", 1);
        CharacterStatisticsController.hasDoubleJump = true;
        
        PlayerPrefs.SetFloat("primaryMultiplierFromSecondary", PlayerPrefs.GetFloat("primaryMultiplierFromSecondary") + primaryMultiplierFromSecondary);
        PlayerPrefs.SetInt("currentMaxHealth", PlayerPrefs.GetInt("currentMaxHealth") + HPIncrease);
        CharacterStatisticsController.baseMaxHealth += HPIncrease;
        CharacterStatisticsController.maxhealth += HPIncrease;
        CharacterStatisticsController.currentHealth += HPIncrease;
        PlayerPrefs.Save();
        UIController.instance.healthBar.maxValue = CharacterStatisticsController.maxhealth;
        UIController.instance.healthBar.currentPercent = CharacterStatisticsController.currentHealth;
    }
}
