using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float timer;
    public GameObject explosionVFX;
    public int damage;
    public float range;
    public GameObject explosionSound;
    float detonateTime;

    private void Awake()
    {
        detonateTime = Time.time + timer;
    }

    /// <summary>
    /// Checks whether the time delay has passed, before damaging any players within range, playing sounds and VFX and deleting itself.
    /// </summary>
    private void Update()
    {
        if(Time.time > detonateTime)
        {
            Instantiate(explosionVFX, transform.position, transform.rotation);
            Instantiate(explosionSound, transform.position, transform.rotation);
            if(Vector3.Distance(transform.position, CharacterCombatController.instance.transform.position) < range)
            {
                CharacterCombatController.instance.DamageHealth(damage);
            }
            Destroy(gameObject);
        }
    }
}
