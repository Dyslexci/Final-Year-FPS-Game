using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static RoomController instance;

    public GameObject rewardPrefab;
    public GameObject bossPrefab;
    public bool roomCleared = false;
    public int totalNumEnemies = 0;

    public string rewardCategory;
    public Room[] rooms;
    DoorController[] exitDoors;
    EnemyManager enemyManager;
    //Transform rewardPlacementTransform;

    private void Awake()
    {
        instance = this;
    }

    public void Initialise(Room[] _rooms, EnemyManager _enemyManager, GameObject[] _exitDoors, string _rewardCategory)
    {
        rooms = _rooms;
        enemyManager = _enemyManager;
        exitDoors = new DoorController[_exitDoors.Length];
        //rewardPlacementTransform = rooms[rooms.Length - 1].rewardSpawnPoint;
        for(int i = 0; i < exitDoors.Length; i++)
        {
            exitDoors[i] = _exitDoors[i].GetComponent<DoorController>();
        }
        rewardCategory = _rewardCategory;
    }

    public void ResetValues()
    {
        rooms = null;
        enemyManager = null;
    }

    /// <summary>
    /// Spawns the initial wave of enemies in the given rooms.
    /// </summary>
    public void InitialSpawn()
    {
        // choose enemies to spawn, between 1-3 different types each room
        int numOfTypesPresent = Random.Range(1, 3);
        // each enemy type is represented by an integer, referencing its location in an array in EnemyManager
        int[] enemyTypesArray = new int[numOfTypesPresent];
        for(int i = 0; i < numOfTypesPresent; i++)
        {
            enemyTypesArray[i] = Random.Range(0, enemyManager.enemyArray.Length - 1);
        }

        // choose how many enemies to spawn, somehow related to the enemy type
        // array which holds the number of each enemy type to spawn, sorted according to the enemy type array
        int[] numberOfEnemiesToSpawn = new int[numOfTypesPresent];
        int totalNum = 0;
        for(int i = 0; i < numberOfEnemiesToSpawn.Length; i++)
        {
            numberOfEnemiesToSpawn[i] = Mathf.RoundToInt(Random.Range(enemyManager.minSpawnArray[enemyTypesArray[i]], enemyManager.maxSpawnArray[enemyTypesArray[i]]) * (Mathf.InverseLerp(0, 21, CharacterStatisticsController.currentRoom) + .75f));
            //print("Chosen to spawn " + numberOfEnemiesToSpawn[i] + " enemies of type " + enemyTypesArray[i] + " - " + enemyManager.enemyArray[enemyTypesArray[i]].GetComponent<Enemy>().enemyName);
            totalNum += numberOfEnemiesToSpawn[i];
        }
        totalNumEnemies = totalNum;

        // pick spawn locations for the enemies

        List<Transform> spawnPointList = GenerateSpawnPointList();

        for(int i = 0; i < enemyTypesArray.Length; i++)
        {
            for(int x = 0; x < numberOfEnemiesToSpawn[i]; x++)
            {
                if(spawnPointList.Count <= 0)
                {
                    spawnPointList = GenerateSpawnPointList();
                }
                int randomIndex = Random.Range(0, spawnPointList.Count);
                Transform currentPoint = spawnPointList[randomIndex];
                GameObject enemy = Instantiate(enemyManager.enemyArray[enemyTypesArray[i]], currentPoint.position, currentPoint.rotation);
                enemy.GetComponent<Enemy>().initialise(2, this, currentPoint, GameController.instance.AIRespawnBin);
                spawnPointList.RemoveAt(randomIndex);
            }
        }
        totalNumEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        print("Number of enemies spawned: " + totalNumEnemies);
    }

    /// <summary>
    /// Returns a list of every spawn point in all provided room components.
    /// </summary>
    /// <returns></returns>
    List<Transform> GenerateSpawnPointList()
    {
        List<Transform> spawnPointList = new List<Transform>();
        for (int i = 0; i < rooms.Length; i++)
        {
            for (int x = 0; x < rooms[i].spawnPoints.Length; x++)
            {
                spawnPointList.Add(rooms[i].spawnPoints[x]);
            }
        }
        return spawnPointList;
    }

    public void SpawnBoss()
    {
        GameObject boss = Instantiate(bossPrefab, rooms[0].spawnPoints[1].position, rooms[0].spawnPoints[1].rotation);
        boss.GetComponent<Enemy>().initialise(0, this, rooms[0].spawnPoints[1], GameController.instance.AIRespawnBin);
        totalNumEnemies = 1;
    }

    /// <summary>
    /// Removes one enemy from the list of enemies - when an enemy dies, it is logged here.
    /// </summary>
    /// <param name="enemyPos"></param>
    public void RemoveEnemy(Vector3 enemyPos)
    {
        totalNumEnemies -= 1;
        CharacterStatisticsController.kills += 1;
        if (totalNumEnemies <= 0)
        {
            EndRoomSequence(enemyPos);
            
        }
    }

    /// <summary>
    /// Performs all necessary actions to setup the end state of the room
    /// </summary>
    /// <param name="lootDropPos"></param>
    public void EndRoomSequence(Vector3 lootDropPos)
    {
        roomCleared = true;
        Vector3 newPos = lootDropPos;
        // spawn in the upgrade node for the player
        RaycastHit hit;
        if(Physics.Raycast(lootDropPos, Vector3.down, out hit, LayerMask.GetMask("Ground")))
        {
            newPos = hit.point;
            newPos.y += .15f;
        }

        GameObject reward = Instantiate(rewardPrefab, newPos, transform.rotation);
        
        print("Room cleared of enemies");
    }

    /// <summary>
    /// Spawns the reward at the location of the last killed enemy
    /// </summary>
    /// <param name="pos"></param>
    public void SpawnReward(Vector3 pos)
    {
        Vector3 newPos = pos;
        // spawn in the upgrade node for the player
        RaycastHit hit;
        if (Physics.Raycast(pos, Vector3.down, out hit, LayerMask.GetMask("Ground")))
        {
            newPos = hit.point;
            newPos.y += .15f;
        }
        Instantiate(rewardPrefab, newPos, transform.rotation);
    }

    public void UnlockDoors()
    {
        foreach (DoorController door in exitDoors)
        {
            door.active = true;
        }
    }
}
