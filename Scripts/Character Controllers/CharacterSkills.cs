using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkills : MonoBehaviour
{
    public float dashLength;
    public float dashDuration;
    public float dashCooldown;

    CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Dashes the player a certain number of units in the current movement direction, independent from the look direction
    /// </summary>
    /// <param name="currMovement"></param>
    public void Dash(Vector3 currMovement)
    {
        controller.Move(currMovement * dashLength);
    }
}
