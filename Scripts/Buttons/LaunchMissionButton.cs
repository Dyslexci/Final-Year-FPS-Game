using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchMissionButton : Interactible
{
    public override void Interact()
    {
        SceneManager.LoadScene("Modules");
    }
}
