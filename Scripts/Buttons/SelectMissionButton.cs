using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMissionButton : Interactible
{
    public string mission;
    public GameObject selectIndicator;
    bool selected;

    private void Awake()
    {
        selectIndicator.SetActive(false);
    }

    public override void Interact()
    {
        print(mission + " selected");
        LaunchMissionController.instance.SelectButton(this);

    }

    public void SetActive()
    {
        selected = true;
        selectIndicator.SetActive(true);
    }

    public void SetInactive()
    {
        selected = false;
        selectIndicator.SetActive(false);
    }
}
