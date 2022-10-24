using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject swarmerPrefab;
    public GameObject smasherPrefab;
    public GameObject holderPrefab;
    public GameObject shooterPrefab;
    public GameObject lvl3missilebotPrefab;
    public GameObject CrabBot;

    public GameObject[] enemyArray;
    // the minimum number of each enemy type which can spawn at any one time
    public int[] minSpawnArray;
    // the maximum number of each enemy type which can spawn at any one time
    public int[] maxSpawnArray;

    private void Awake()
    {
        enemyArray = new GameObject[6];
        enemyArray[0] = swarmerPrefab;
        enemyArray[1] = smasherPrefab;
        enemyArray[2] = holderPrefab;
        enemyArray[3] = shooterPrefab;
        enemyArray[4] = lvl3missilebotPrefab;
        enemyArray[5] = CrabBot;
        minSpawnArray = new int[6] { 2, 1, 3, 4 , 2, 5};
        maxSpawnArray = new int[6] { 4, 2, 6, 7 , 4, 10};
    }
}
