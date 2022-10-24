using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class GenerateRandomRoom : MonoBehaviour
{
    public static GenerateRandomRoom instance;

    public NavMeshBaker baker;
    [SerializeField] Rooms[] RoomSets;
    [SerializeField] GameObject shopRoomPrefab;
    [SerializeField] GameObject bossRoomPrefab;
    public Transform generationPoint;
    public Room[] activeRooms;

    public GameObject entryTeleportRoomPrefab;
    public GameObject exitTeleportRoomPrefab;
    public GameObject doorPrefab;

    GameObject primary;
    GameObject secondary;
    public GameObject teleportRoom;
    GameObject entryDoor;
    public GameObject[] exitDoors;
    public GameObject[] exitTeleportRooms;

    Room currentRoom;
    public TeleportRoom entryTeleportRoom;
    public GameObject[] currentRoomSet;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        activeRooms = new Room[2];
    }

    /// <summary>
    /// Removes the existing room in preparation for a new room.
    /// </summary>
    public void DestroyCurrentRoom()
    {
        Destroy(primary);
        Destroy(secondary);
        Destroy(teleportRoom);
        Destroy(entryDoor);
        foreach (GameObject t in exitDoors) Destroy(t);
        foreach (GameObject t in exitTeleportRooms) Destroy(t);
    }

    /// <summary>
    /// Generate the room - spawns in an entrance room and doorway, the two halves of the room, and the two exit doorways and teleport rooms, along with assigning the
    /// options for rewards for the following rooms. Every 5th room a shop is always an option.
    /// </summary>
    public void GenerateRoom(string roomType)
    {
        if(roomType.Equals("Shop"))
        {
            GenerateShopRoom();
            return;
        }
        if(roomType.Equals("Boss"))
        {
            GenerateBossRoom();
            return;
        }
        GenerateCombatRoom();
    }

    /// <summary>
    /// Places components in a way that the rooms are non-intersecting and lined up seamlessly, sets up doors so that the player cannot just walk through the end, and generates
    /// the available rewards for the two options of room.
    /// </summary>
    void GenerateCombatRoom()
    {
        // Spawns in the entrance teleport room and door
        List<string> upgradeCategories = new List<string>(UpgradesController.instance.upgradeCategories);
        teleportRoom = Instantiate(entryTeleportRoomPrefab, generationPoint.position, generationPoint.rotation);
        entryTeleportRoom = teleportRoom.GetComponent<TeleportRoom>();
        entryDoor = Instantiate(doorPrefab, entryTeleportRoom.exitPoint.position, entryTeleportRoom.exitPoint.rotation);
        DoorController _door = entryDoor.GetComponent<DoorController>();
        RepositionTransforms(entryTeleportRoom.exitPoint.position, _door.entryPoint.position, entryDoor);
        _door.EnableLights();
        _door.autoClose = true;

        // Selects the current set of rooms from the available pieces
        Rooms currentSet = RoomSets[Random.Range(0, RoomSets.Length)];
        primary = currentSet.primaryRoomPrefabs[Random.Range(0, currentSet.primaryRoomPrefabs.Length)];
        secondary = currentSet.secondaryRoomPrefabs[Random.Range(0, currentSet.secondaryRoomPrefabs.Length)];

        // Generates the room components chosen previously
        primary = Instantiate(primary, _door.exitPoint.position, _door.exitPoint.rotation);
        currentRoom = primary.GetComponent<Room>();
        activeRooms[0] = currentRoom;
        RepositionTransforms(_door.exitPoint.position, currentRoom.entryPoint.position, primary);
        secondary = Instantiate(secondary, primary.transform.position, currentRoom.transform.rotation);
        RepositionTransforms(currentRoom.exitPoint[0].position, secondary.GetComponent<Room>().entryPoint.position, secondary);
        currentRoom = secondary.GetComponent<Room>();
        activeRooms[1] = currentRoom;

        // Create the exit doors in the given points, assigns them with a randomly chosen upgrade path and locks them
        exitDoors = new GameObject[currentRoom.exitPoint.Length];
        exitTeleportRooms = new GameObject[currentRoom.exitPoint.Length];
        for (int i = 0; i < currentRoom.exitPoint.Length; i++)
        {
            int randomUpgrade = Random.Range(0, upgradeCategories.Count);
            exitDoors[i] = Instantiate(doorPrefab, currentRoom.exitPoint[i].position, currentRoom.exitPoint[i].rotation);
            DoorController _currentDoor = exitDoors[i].GetComponent<DoorController>();
            _currentDoor.EnableLights();
            _currentDoor.active = false;
            _currentDoor.autoClose = false;

            Quaternion rot = (_currentDoor.exitPoint.rotation * Quaternion.Euler(Vector3.up * 45));
            RepositionTransforms(currentRoom.exitPoint[i].position, _currentDoor.entryPoint.position, exitDoors[i]);
            exitTeleportRooms[i] = Instantiate(exitTeleportRoomPrefab, _currentDoor.exitPoint.position,
                rot);
            RepositionTransforms(_currentDoor.exitPoint.position, exitTeleportRooms[i].GetComponent<Room>().entryPoint.position, exitTeleportRooms[i]);
            if (i == 0)
            {
                if (CharacterStatisticsController.currentRoom % 5 == 0 && CharacterStatisticsController.currentRoom != 0)
                {
                    print(CharacterStatisticsController.currentRoom);
                    _currentDoor.InitialiseUpgradeDisplay("Shop");
                    exitTeleportRooms[i].GetComponent<Room>().InitialiseUpgradePath("Shop");
                }
                else
                {
                    _currentDoor.InitialiseUpgradeDisplay(upgradeCategories.ToArray()[randomUpgrade]);
                    exitTeleportRooms[i].GetComponent<Room>().InitialiseUpgradePath(upgradeCategories.ToArray()[randomUpgrade]);
                    upgradeCategories.RemoveAt(randomUpgrade);
                }
            }
            else
            {
                _currentDoor.InitialiseUpgradeDisplay(upgradeCategories.ToArray()[randomUpgrade]);
                exitTeleportRooms[i].GetComponent<Room>().InitialiseUpgradePath(upgradeCategories.ToArray()[randomUpgrade]);
                upgradeCategories.RemoveAt(randomUpgrade);
            }
        }
        // Builds the NavMesh used by AI
        NavMeshSurface[] surfaces = primary.GetComponents<NavMeshSurface>();
        foreach(NavMeshSurface mesh in surfaces)
        {
            mesh.BuildNavMesh();
        }
    }

    /// <summary>
    /// Generates the shop room and populates it with buyables in the stalls - common code with the room generation method.
    /// </summary>
    void GenerateShopRoom()
    {
        List<string> upgradeCategories = UpgradesController.instance.upgradeCategories;
        teleportRoom = Instantiate(entryTeleportRoomPrefab, generationPoint.position, generationPoint.rotation);
        entryTeleportRoom = teleportRoom.GetComponent<TeleportRoom>();
        entryDoor = Instantiate(doorPrefab, entryTeleportRoom.exitPoint.position, entryTeleportRoom.exitPoint.rotation);
        DoorController _door = entryDoor.GetComponent<DoorController>();
        RepositionTransforms(entryTeleportRoom.exitPoint.position, _door.entryPoint.position, entryDoor);
        _door.EnableLights();

        primary = Instantiate(shopRoomPrefab, _door.exitPoint.position, _door.exitPoint.rotation);
        currentRoom = primary.GetComponent<Room>();
        activeRooms[0] = currentRoom;
        primary.transform.Rotate(0, 90, 0);
        RepositionTransforms(_door.exitPoint.position, currentRoom.entryPoint.position, primary);
        secondary = primary;
        currentRoom = secondary.GetComponent<Room>();
        activeRooms[1] = currentRoom;

        exitDoors = new GameObject[currentRoom.exitPoint.Length];
        exitTeleportRooms = new GameObject[currentRoom.exitPoint.Length];
        int randomUpgrade = Random.Range(0, upgradeCategories.Count);
        exitDoors[0] = Instantiate(doorPrefab, currentRoom.exitPoint[0].position, currentRoom.exitPoint[0].rotation);
        DoorController _currentDoor = exitDoors[0].GetComponent<DoorController>();
        _currentDoor.EnableLights();
        _currentDoor.active = false;
        _currentDoor.autoClose = false;

        Quaternion rot = (_currentDoor.exitPoint.rotation * Quaternion.Euler(Vector3.up * 45));
        RepositionTransforms(currentRoom.exitPoint[0].position, _currentDoor.entryPoint.position, exitDoors[0]);
        exitTeleportRooms[0] = Instantiate(exitTeleportRoomPrefab, _currentDoor.exitPoint.position,
            rot);
        RepositionTransforms(_currentDoor.exitPoint.position, exitTeleportRooms[0].GetComponent<Room>().entryPoint.position, exitTeleportRooms[0]);

        _currentDoor.InitialiseUpgradeDisplay(upgradeCategories.ToArray()[randomUpgrade]);
        exitTeleportRooms[0].GetComponent<Room>().InitialiseUpgradePath(upgradeCategories.ToArray()[randomUpgrade]);
    }

    /// <summary>
    /// Spawns in the boss room.
    /// </summary>
    void GenerateBossRoom()
    {
        primary = Instantiate(bossRoomPrefab, generationPoint.position, generationPoint.rotation);
        entryTeleportRoom = primary.GetComponent<TeleportRoom>();
        secondary = primary;
        exitDoors = new GameObject[1];
        NavMeshSurface[] surfaces = primary.GetComponents<NavMeshSurface>();
        foreach (NavMeshSurface mesh in surfaces)
        {
            mesh.BuildNavMesh();
        }
    }

    /// <summary>
    /// Corrects for any mis-alignment in the prefab or room models.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="connection"></param>
    /// <param name="objectToMove"></param>
    void RepositionTransforms(Vector3 origin, Vector3 connection, GameObject objectToMove)
    {
        Vector3 delta = origin - connection;
        objectToMove.transform.position += delta;
    }
}
