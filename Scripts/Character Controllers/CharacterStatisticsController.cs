using UnityEngine;

public static class CharacterStatisticsController
{
    // base values which will be modifiable outside of a run, and will be the starting values for each run
    public static int baseMaxHealth = 100;

    public static int baseNumberOfDashes = 2;

    public static int baseMaxNumberOfLives = 1;

    public static float baseCritChance = 1;
    public static float baseCritMultiplier = 2.0f;

    public static float basePrimaryMaxDamage = 60;
    public static float basePrimaryMinDamage = 20;
    public static float basePrimaryCritDamage = 70;
    public static float baseCritDuration = 0.3f;

    public static float baseSecondaryDamage = 10;
    public static int baseSecondaryNumberOfShots = 9;
    public static float baseSecondaryAttackAngle = 35;

    // modifiable values the game uses during a run
    public static float sensitivity;
    public static bool screenShake;
    public static bool godMode;

    public static int currentRoom;
    public static int numberOfRooms;
    public static int attemptNumber;
    public static int kills;

    public static int maxhealth;
    public static int currentHealth;
    public static int numberofDashes;
    public static int maxNumberOfLives;
    public static int currentLives;
    public static float critChance;
    public static float critMultiplier;

    public static int shards;
    public static int credits;

    public static float primaryMaxDamage;
    public static float primaryMinDamage;
    public static float primaryCritDamage;
    public static float critDuration;

    public static float secondaryDamage;
    public static int secondaryNumberOfShots;
    public static float secondaryAttackAngle;

    public static float secondaryMultiplierFromPrimary;
    public static float primaryMultiplierFromSecondary;

    public static bool hasDoubleJump;
    
    public static void InitialiseBaseStatistics(bool deadCase)
    {
        if (PlayerPrefs.HasKey("baseMaxHealth"))
            baseMaxHealth = PlayerPrefs.GetInt("baseMaxHealth");
        maxhealth = baseMaxHealth;
        currentHealth = baseMaxHealth;
        numberofDashes = baseNumberOfDashes;
        maxNumberOfLives = baseMaxNumberOfLives;
        currentLives = baseMaxNumberOfLives;
        critChance = baseCritChance;
        critMultiplier = baseCritMultiplier;
        primaryMaxDamage = basePrimaryMaxDamage;
        primaryMinDamage = basePrimaryMinDamage;
        primaryCritDamage = basePrimaryCritDamage;
        critDuration = baseCritDuration;
        secondaryDamage = baseSecondaryDamage;
        secondaryNumberOfShots = baseSecondaryNumberOfShots;
        secondaryAttackAngle = baseSecondaryAttackAngle;
        hasDoubleJump = false;

        if(deadCase)
        {
            credits = 0;
            currentRoom = 0;
            return;
        }
        if(PlayerPrefs.HasKey("shards"))
        {
            shards = PlayerPrefs.GetInt("shards");
            kills = PlayerPrefs.GetInt("kills");
            attemptNumber = PlayerPrefs.GetInt("attemptNumber");
            secondaryMultiplierFromPrimary = PlayerPrefs.GetFloat("secondaryMultiplierFromPrimary");
            primaryMultiplierFromSecondary = PlayerPrefs.GetFloat("primaryMultiplierFromSecondary");
            baseMaxHealth = PlayerPrefs.GetInt("baseMaxHealth");
        } else
        {
            shards = 0;
            kills = 0;
            attemptNumber = 0;
            secondaryMultiplierFromPrimary = 1;
            primaryMultiplierFromSecondary = 1;
            
        }
        credits = 0;
        currentRoom = 0;
    }

    public static void InitialiseFromPlayerPrefs()
    {
        numberOfRooms = PlayerPrefs.GetInt("numberOfRooms");
        currentRoom = PlayerPrefs.GetInt("currentRoom");
        maxhealth = PlayerPrefs.GetInt("currentMaxHealth");
        currentHealth = PlayerPrefs.GetInt("currentHealth");
        numberofDashes = PlayerPrefs.GetInt("currentNumberOfDashes");
        maxNumberOfLives = PlayerPrefs.GetInt("currentNumberOfLives");
        currentLives = PlayerPrefs.GetInt("currentLives");
        critChance = PlayerPrefs.GetFloat("currentCritChance");
        critMultiplier = PlayerPrefs.GetFloat("currentCritMultiplier");
        primaryMaxDamage = PlayerPrefs.GetFloat("currentPrimaryMaxDamage");
        primaryMinDamage = PlayerPrefs.GetFloat("currentPrimaryMinDamage");
        primaryCritDamage = PlayerPrefs.GetFloat("currentPrimaryCritDamage");
        critDuration = PlayerPrefs.GetFloat("currentCritDuration");
        secondaryDamage = PlayerPrefs.GetFloat("currentSecondaryDamage");
        secondaryNumberOfShots = PlayerPrefs.GetInt("currentSecondaryNumberOfShots");
        secondaryAttackAngle = PlayerPrefs.GetFloat("currentSecondaryAttackAngle");
        shards = PlayerPrefs.GetInt("shards");
        credits = PlayerPrefs.GetInt("currentCredits");
        kills = PlayerPrefs.GetInt("kills");
        attemptNumber = PlayerPrefs.GetInt("attemptNumber");
        secondaryMultiplierFromPrimary = PlayerPrefs.GetFloat("secondaryMultiplierFromPrimary");
        primaryMultiplierFromSecondary = PlayerPrefs.GetFloat("primaryMultiplierFromSecondary");
        baseMaxHealth = PlayerPrefs.GetInt("baseMaxHealth");
        if(PlayerPrefs.HasKey("hasDoubleJump"))
        {
            hasDoubleJump = PlayerPrefs.GetInt("hasDoubleJump") == 1 ? true : false;
        } else
        {
            PlayerPrefs.SetInt("hasDoubleJump", 0);
            hasDoubleJump = false;
        }
        

    }

    public static void SaveGameState()
    {
        PlayerPrefs.SetInt("numberOfRooms", numberOfRooms);
        PlayerPrefs.SetInt("currentRoom", currentRoom);
        PlayerPrefs.SetInt("currentMaxHealth", maxhealth);
        PlayerPrefs.SetInt("currentHealth", currentHealth);
        PlayerPrefs.SetInt("currentNumberOfDashes", numberofDashes);
        PlayerPrefs.SetInt("currentNumberOfLives", maxNumberOfLives);
        PlayerPrefs.SetInt("currentLives", currentLives);
        PlayerPrefs.SetFloat("currentCritChance", critChance);
        PlayerPrefs.SetFloat("currentCritMultiplier", critMultiplier);
        PlayerPrefs.SetFloat("currentPrimaryMaxDamage", primaryMaxDamage);
        PlayerPrefs.SetFloat("currentPrimaryMinDamage", primaryMinDamage);
        PlayerPrefs.SetFloat("currentPrimaryCritDamage", primaryCritDamage);
        PlayerPrefs.SetFloat("currentCritDuration", critDuration);
        PlayerPrefs.SetFloat("currentSecondaryDamage", secondaryDamage);
        PlayerPrefs.SetInt("currentSecondaryNumberOfShots", secondaryNumberOfShots);
        PlayerPrefs.SetFloat("currentSecondaryAttackAngle", secondaryAttackAngle);
        PlayerPrefs.SetInt("shards", shards);
        PlayerPrefs.SetInt("currentCredits", credits);
        PlayerPrefs.SetInt("kills", kills);
        PlayerPrefs.SetInt("attemptNumber", attemptNumber);
        PlayerPrefs.SetFloat("secondaryMultiplierFromPrimary", secondaryMultiplierFromPrimary);
        PlayerPrefs.SetFloat("primaryMultiplierFromSecondary", primaryMultiplierFromSecondary);
        PlayerPrefs.SetInt("baseMaxHealth", baseMaxHealth);
        PlayerPrefs.SetInt("hasDoubleJump", hasDoubleJump == true ? 1 : 0);
        PlayerPrefs.Save();
    }
}
