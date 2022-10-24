using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains data about a room - attached to the parent object of a room prefab
/// </summary>
public class Room : MonoBehaviour
{
    public string roomType;
    public Transform entryPoint;
    public Transform[] exitPoint;
    public Vector3 rotation;

    public Transform[] spawnPoints;
    public Transform rewardSpawnPoint;
    public string upgradepath;

    public void InitialiseUpgradePath(string upgradeCategory)
    {
        upgradepath = upgradeCategory;
    }
}
