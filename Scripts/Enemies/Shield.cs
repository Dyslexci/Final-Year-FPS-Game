using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public int maxHealth = 50;
    public MeshRenderer[] meshRenderer;
    Material[] shieldMaterials;
    [ColorUsage(true, true)]
    public Color baseCol;
    [ColorUsage(true, true)]
    public Color damagedCol;
    [ColorUsage(true, true)]
    public Color criticallyDamagedCol;
    int currentHealth = 50;


    private void Awake()
    {
        currentHealth = maxHealth;
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
            shieldMaterials = materialList.ToArray();
        }
    }

    /// <summary>
    /// Allows the shield to be damaged by the same method as the enemies, to utilise the message event system.
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(int damage)
    {
        if(damage < currentHealth)
        {
            currentHealth -= damage;
            if(currentHealth >= maxHealth / 2)
            {
                foreach(Material mat in shieldMaterials)
                {
                    mat.SetColor("ShieldBaseColour", damagedCol);
                }
            } else
            {
                foreach (Material mat in shieldMaterials)
                {
                    mat.SetColor("ShieldBaseColour", criticallyDamagedCol);
                }
            }
            return;
        }
        Destroy(gameObject);
    }
}
