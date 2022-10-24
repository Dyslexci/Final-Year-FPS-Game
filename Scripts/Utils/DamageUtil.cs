using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class - messages can only send a single parameter, so this allows the weapon to send a damage message providing damage and damage types in one.
/// </summary>
public class DamageUtil
{
    public int damage;
    public string type;

    public DamageUtil(int _damage, string _type)
    {
        damage = _damage;
        type = _type;
    }
}
