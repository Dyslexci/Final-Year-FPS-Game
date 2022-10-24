using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Interactible
{
    private void Awake()
    {
        toolTip = "Heal for <color=#97FF31FF>+25</color> points!";
    }

    public override void Interact()
    {
        CharacterCombatController.instance.HealHealth(25);
        Destroy(gameObject);
    }
}
