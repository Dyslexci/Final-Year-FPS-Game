using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    public float timeToStartDamage = 5.0f;
    public float damagePeriod = 4.0f;
    public int damage;
    public MeshRenderer[] meshRenderer;
    Material[] materials;
    [ColorUsage(true, true)]
    public Color baseCol;
    [ColorUsage(true, true)]
    public Color damageCol;

    bool zoneTriggered = false;
    bool damaging;
    bool playerTouching;
    float delay = 1f;
    float timeToDamage = 0;

    private void Awake()
    {
        List<Material> materialList = new List<Material>();
        if (meshRenderer.Length > 0)
        {
            foreach (MeshRenderer renderer in meshRenderer)
            {
                foreach (Material mat in renderer.materials)
                {
                    materialList.Add(mat);
                }
            }
            materials = materialList.ToArray();
        }
        foreach (Material mat in materials)
        {
            mat.SetColor("Line_Tint", baseCol);
        }
    }

    /// <summary>
    /// Damages the player for every period of time, while they are touching this ground.
    /// </summary>
    private void FixedUpdate()
    {
        if (Time.time > timeToDamage && playerTouching && damaging && zoneTriggered)
        {
            timeToDamage = Time.time + delay;
            CharacterCombatController.instance.DamageHealth(damage);
        }
    }

    public void PlayerEnteredZone()
    {
        if(!zoneTriggered)
        {
            zoneTriggered = true;
            StartCoroutine(CountdownToDamage());
        }
    }

    /// <summary>
    /// Counts down before enabling the floor to deal damage and changing the floor colour.
    /// </summary>
    /// <returns></returns>
    IEnumerator CountdownToDamage()
    {
        yield return new WaitForSeconds(timeToStartDamage);
        foreach (Material mat in materials)
        {
            mat.SetColor("Line_Tint", damageCol);
        }
        damaging = true;

        while(playerTouching)
            yield return new WaitForSeconds(damagePeriod);

        foreach (Material mat in materials)
        {
            mat.SetColor("Line_Tint", baseCol);
        }
        damaging = false;
        zoneTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerEnteredZone();
            playerTouching = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerTouching = false;
        }
    }
}
