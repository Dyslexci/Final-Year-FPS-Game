using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the slightly different room data for the teleporter rooms - applied to the parent object of the room prefab
/// </summary>
public class TeleportRoom : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform exitPoint;
    public Transform teleportPoint;
}
