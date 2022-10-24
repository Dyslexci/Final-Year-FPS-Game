using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class providing the Interact method to any interactible object placed in the world.
/// </summary>
public abstract class Interactible : MonoBehaviour
{
    public string toolTip = "Interact";
    public AudioSource interactSound;

    public abstract void Interact();
}
