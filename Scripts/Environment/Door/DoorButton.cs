using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : Interactible
{
    DoorController door;

    private void Awake()
    {
        door = gameObject.GetComponentInParent<DoorController>();
    }

    private void Update()
    {
        if (door.open)
        {
            toolTip = "Close Door";
            return;
        }
        toolTip = "Open Door";        
    }

    public override void Interact()
    {
        if (!door.active) return;
        interactSound.Play();

        if(door.open)
        {
            door.CloseDoor();
            return;
        }
        door.OpenDoor();
    }
}
