using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public OptionsManager optionsManager;
    public GameObject player;
    public Transform generationPoint, AIRespawnBin;
    public bool paused;
    public bool playerDead;

    GenerateRandomRoom roomGenerator;
    public Vector3 teleportPoint;

    RoomController roomController;
    EnemyManager enemyManager;

    private void Awake()
    {       
        instance = this;
        Application.targetFrameRate = 144;
        Cursor.lockState = CursorLockMode.None;
        roomController = GetComponent<RoomController>();
        roomGenerator = GetComponent<GenerateRandomRoom>();
        enemyManager = GetComponent<EnemyManager>();
        Physics.IgnoreLayerCollision(9, 8, true);
        if(PlayerPrefs.HasKey("InRun"))
        {
            bool inRun = PlayerPrefs.GetInt("InRun") == 1 ? true : false;
            if(inRun)
            {
                CharacterStatisticsController.InitialiseFromPlayerPrefs();
            } else
            {
                InitialiseNewGame();
                CharacterStatisticsController.InitialiseBaseStatistics(false);
            }
        } else
        {
            InitialiseNewGame();
            CharacterStatisticsController.InitialiseBaseStatistics(false);
            CharacterStatisticsController.SaveGameState();
        }
        PlayerPrefs.SetInt("InRun", 1);
        PlayerPrefs.Save();
        print(PlayerPrefs.GetInt("baseMaxHealth") + " max health");
    }

    void InitialiseNewGame()
    {
        CharacterStatisticsController.numberOfRooms = Random.Range(13, 18);
        CharacterStatisticsController.currentRoom = 0;
    }

    void Start()
    {
        optionsManager.InitialiseSavedOptions();
        roomGenerator.GenerateRoom("");
        teleportPoint = roomGenerator.entryTeleportRoom.teleportPoint.position;
        int randomUpgrade = Random.Range(0, UpgradesController.instance.upgradeCategories.Count);
        player.transform.position = teleportPoint;
        //roomController.Initialise(roomGenerator.activeRooms, enemyManager, roomGenerator.exitDoors, UpgradesController.instance.upgradeCategories.ToArray()[randomUpgrade]);
        roomController.Initialise(roomGenerator.activeRooms, enemyManager, roomGenerator.exitDoors, "Attack");
        InitialiseGame();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void InitialiseGame()
    {
        player.transform.position = teleportPoint;
        roomController.InitialSpawn();
        TutorialManager.instance.DisplayTutorial();
    }

    public AudioSource teleportSFX;

    /// <summary>
    /// Generates a new room, spawns enemies and picks rewards.
    /// </summary>
    /// <param name="upgradeCategory"></param>
    public void ChangeRoom(string upgradeCategory)
    {
        roomGenerator.DestroyCurrentRoom();
        CharacterStatisticsController.currentRoom += 1;
        if(upgradeCategory.Equals("Shop"))
        {
            roomGenerator.GenerateRoom("Shop");
            int randomCategory = Random.Range(0, UpgradesController.instance.upgradeCategories.Count);
            string chosenUpgrade = UpgradesController.instance.upgradeCategories.ToArray()[randomCategory];
            roomController.Initialise(roomGenerator.activeRooms, enemyManager, roomGenerator.exitDoors, chosenUpgrade);
            roomController.SpawnReward(roomGenerator.activeRooms[0].spawnPoints[0].position);
            roomController.UnlockDoors();
        } else if(CharacterStatisticsController.currentRoom >= 16)
        {
            roomGenerator.GenerateRoom("Boss");
            roomController.Initialise(roomGenerator.activeRooms, enemyManager, roomGenerator.exitDoors, "boss");
            roomController.SpawnBoss();
        } else
        {
            roomGenerator.GenerateRoom("");
            roomController.Initialise(roomGenerator.activeRooms, enemyManager, roomGenerator.exitDoors, upgradeCategory);
            roomController.InitialSpawn();
        }
        StartCoroutine(LoopTeleportPlayer(roomGenerator.entryTeleportRoom.teleportPoint.position));
        Debug.LogError(player.transform.position);
        player.transform.position = roomGenerator.entryTeleportRoom.teleportPoint.position;
        Debug.LogError(player.transform.position);
        teleportSFX.Play();
        TutorialManager.instance.DisplayTutorial();
        Unpause();
        Debug.LogError("Now in room: " + CharacterStatisticsController.currentRoom);
    }

    /// <summary>
    /// Teleports player to the start position every frame for a second, to circumvent some weird bug where the player object will not have its position changed in a single
    /// frame.
    /// </summary>
    /// <param name="newPos"></param>
    /// <returns></returns>
    IEnumerator LoopTeleportPlayer(Vector3 newPos)
    {
        float timeToLoop = Time.time + 1;
        while(Time.time < timeToLoop)
        {
            yield return new WaitForEndOfFrame();
            player.transform.position = newPos;
        }
    }

    /// <summary>
    /// Pauses the game - the paused variable is used by multiple scripts to pause ongoing actions
    /// </summary>
    public void Pause()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        PauseMenuController.instance.DisplayPauseMenu();
        LeanTween.pauseAll();
    }

    /// <summary>
    /// Pauses the game, but without the pause menu being opened - useful for things like death or upgrades screen.
    /// </summary>
    public void PauseWithoutUI()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        LeanTween.pauseAll();
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void Unpause()
    {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        PauseMenuController.instance.HidePauseMenu(true);
        LeanTween.resumeAll();
    }

    public CanvasGroup fadeToBlack;

    /// <summary>
    /// Plays the player death animation and pauses without UI.
    /// </summary>
    public void EndGame()
    {
        CharacterCombatController.instance.DeathAnim();
        playerDead = true;
        PauseWithoutUI();
        UIController.instance.HideAllUI();
    }

    /// <summary>
    /// Sets player preferences to remove from the 'in run' state, and returns to home.
    /// </summary>
    public void EndGameCalcs()
    {
        CharacterStatisticsController.attemptNumber += 1;
        CharacterStatisticsController.InitialiseBaseStatistics(true);
        CharacterStatisticsController.SaveGameState();
        PlayerPrefs.SetInt("InRun", 0);
        PlayerPrefs.SetInt("DeathEntry", 1);
        SceneManager.LoadScene("HomeArea");
    }

    public void QuitGame()
    {
        CharacterStatisticsController.SaveGameState();
        Application.Quit();
    }
}
