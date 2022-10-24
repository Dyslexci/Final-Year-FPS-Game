using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRoomTeleporterButton : Interactible
{
    public Animator teleportAnimation;
    public Room parentRoom;
    public AudioSource teleportSFX;

    /// <summary>
    /// Triggers end of room teleport sequence on interaction.
    /// </summary>
    public override void Interact()
    {
        GameController.instance.PauseWithoutUI();
        teleportAnimation.SetTrigger("Teleport");
        UIController.instance.EndRoomSequence(parentRoom.upgradepath);
        teleportSFX.Play();
    }
}
