using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The scriptable object parent class, used by all upgrades.
/// </summary>
public abstract class UpgradeEffect : ScriptableObject
{
    public string title;
    [TextArea(15, 20)]
    public string description;
    public int cost;

    public abstract void ApplyEffect();
}
