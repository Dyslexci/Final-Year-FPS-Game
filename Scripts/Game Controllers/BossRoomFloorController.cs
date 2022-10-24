using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class randomly swaps an array of objects between a damaging state and a safe state.
/// </summary>
public class BossRoomFloorController : MonoBehaviour
{
    public GameObject[] floors;
    [ColorUsage(true, true)]
    public Color warningColour;
    [ColorUsage(true, true)]
    public Color damageColour;

    public float minTimeToChange = 20.0f;
    public float maxTimeToChange = 30.0f;
    public bool floorsEnabled = true;
    float nextChangeTime = 0;
    bool sequenceEnded;

    private void Awake()
    {
        nextChangeTime = Time.time + Random.Range(minTimeToChange, maxTimeToChange);
    }

    /// <summary>
    /// Chooses a number of objects from the given array, changes their states to damaging and other objects to safe, and sets a random amount of time to wait.
    /// </summary>
    private void Update()
    {
        if (sequenceEnded) return;
        if (GameController.instance.paused) return;
        if(!floorsEnabled && !sequenceEnded)
        {
            foreach (GameObject floor in this.floors)
            {
                floor.SetActive(false);
            }
            return;
        }

        if(Time.time > nextChangeTime)
        {
            nextChangeTime = Time.time + Random.Range(minTimeToChange, maxTimeToChange);
            int floorsToRemove = Random.Range(1, 2);
            List<GameObject> floors = new List<GameObject>(this.floors);
            for (int i = 0; i < floorsToRemove; i++)
            {
                floors.RemoveAt(Random.Range(0, floors.Count));
            }
            StartCoroutine(ChangeFloors(floors));
        }
    }

    IEnumerator ChangeFloors(List<GameObject> floors)
    {
        foreach (GameObject floor in this.floors)
        {
            floor.SetActive(false);
        }
        foreach (GameObject floor in floors)
        {
            floor.GetComponent<StaticDamage>().damageEnabled = false;
            floor.SetActive(true);
        }
        List<Material> materialList = new List<Material>();
        List<MeshRenderer> meshRenderer = new List<MeshRenderer>();
        foreach(GameObject floor in floors)
        {
            meshRenderer.Add(floor.GetComponent<MeshRenderer>());
        }
        if (meshRenderer.Count > 0)
        {
            foreach (MeshRenderer renderer in meshRenderer)
            {
                foreach (Material mat in renderer.materials)
                {
                    materialList.Add(mat);
                }
            }
        }
        foreach(Material mat in materialList)
        {
            mat.SetColor("Line_Tint", warningColour);
        }
        yield return new WaitForSeconds(1);
        foreach (Material mat in materialList)
        {
            mat.SetColor("Line_Tint", damageColour);
        }
        foreach (GameObject floor in floors)
        {
            floor.GetComponent<StaticDamage>().damageEnabled = true;
        }
    }
}
