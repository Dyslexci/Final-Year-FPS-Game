using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUpgradesDispensorButton : Interactible
{
    public ButtonScriptableobjectIntegration[] buttons;

    public override void Interact()
    {
        PauseMenuController.instance.DisplayHomeUpgradesMenu();
        UpdateButtonInfos();
    }

    public void UpdateButtonInfos()
    {
        foreach (ButtonScriptableobjectIntegration button in buttons)
        {
            button.UpdateInfo();
        }
    }
}
