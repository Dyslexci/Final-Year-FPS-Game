using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropMaster : Interactible
{
    public GameObject parentObj;

    public UpgradeEffect healthUpgrade;
    public UpgradeEffect shardsUpgrade;
    public UpgradeEffect creditUpgrade;
    public List<UpgradeEffect> attackUpgrades;
    public UpgradeEffect boss;

    string upgradeCategory;
    List<UpgradeEffect> selectedUpgrades;
    UpgradeEffect[] selectedUpgradeArray;

    /// <summary>
    /// Determines which upgrade category the drop has been spawned as and modify tooltip appropriately
    /// </summary>
    private void Awake()
    {
        toolTip = "Collect Orb";
        upgradeCategory = RoomController.instance.rewardCategory;
        switch (upgradeCategory)
        {
            case "Health":
                toolTip = "Collect 25 Health";
                break;
            case "Shards":
                toolTip = "Collect 20 Shards";
                break;
            case "Credits":
                toolTip = "Collect 100 Credits";
                break;
            case "Attack":
                toolTip = "Collect Attack Boon";
                break;
            case "boss":
                toolTip = "Restore Harmoncy";
                break;
        }
    }

    public AudioSource collectLootSFX;

    /// <summary>
    /// Perform the correct upgrade
    /// </summary>
    public override void Interact()
    {
        collectLootSFX.Play();
        // do all the decisions about collecting a reward
        switch(upgradeCategory)
        {
            case "Health":
                print("Collecting health");
                healthUpgrade.ApplyEffect();
                RewardCollected();
                break;
            case "Shards":
                print("Collecting shards");
                shardsUpgrade.ApplyEffect();
                RewardCollected();
                break;
            case "Credits":
                print("Collecting credits");
                creditUpgrade.ApplyEffect();
                RewardCollected();
                break;
            case "Attack":
                print("Collecting attack buff");
                DisplayAttackUpgradeUI();
                break;
            case "boss":
                print("Collecting boss drop");
                BossDropCollection();
                break;
        }
    }

    void BossDropCollection()
    {
        CharacterStatisticsController.shards += 200;
        GameController.instance.EndGame();

    }

    void DisplayAttackUpgradeUI()
    {
        selectedUpgrades = new List<UpgradeEffect>();
        for(int i = 0; i < 3; i++)
        {
            int randomUpgrade = Random.Range(0, attackUpgrades.Count);
            selectedUpgrades.Add(attackUpgrades.ToArray()[randomUpgrade]);
            attackUpgrades.RemoveAt(randomUpgrade);
        }
        selectedUpgradeArray = selectedUpgrades.ToArray();
        UIController.instance.PopulateUpgradeDisplay(selectedUpgradeArray[0], selectedUpgradeArray[1], selectedUpgradeArray[2], this);
        PauseMenuController.instance.eventSys.SetSelectedGameObject(PauseMenuController.instance.upgradeMenuButton);
    }

    public GameObject rewardSelectedSFXPrefab;

    /// <summary>
    /// Applies the associated effect of the selected button
    /// </summary>
    /// <param name="num"></param>
    public void UpgradeSelected(int num)
    {
        selectedUpgradeArray[num].ApplyEffect();
        try
        {
            Instantiate(rewardSelectedSFXPrefab, transform.position, transform.rotation);
        }
        catch (System.NullReferenceException)
        {
            Instantiate(rewardSelectedSFXPrefab, GameController.instance.gameObject.transform.position, GameController.instance.gameObject.transform.rotation);
        }
        
        RewardCollected();
    }

    public void RewardCollected()
    {
        RoomController.instance.UnlockDoors();
        Destroy(parentObj);
    }
}
