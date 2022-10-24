using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticDamage : MonoBehaviour
{
    public int damage = 3;
    public bool damageEnabled = true;
    bool playerTouching;
    float delay = 1f;
    float timeToDamage = 0;

    /// <summary>
    /// Deals damage while the player is stood on the object, with time intervals between damage.
    /// </summary>
    private void FixedUpdate()
    {
        if (!damageEnabled) return;

        if(Time.time > timeToDamage && playerTouching)
        {
            timeToDamage = Time.time + delay;
            CharacterCombatController.instance.DamageHealth(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTouching = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTouching = false;
        }
    }
}
