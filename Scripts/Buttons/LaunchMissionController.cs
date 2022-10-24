using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LaunchMissionController : MonoBehaviour
{
    public static LaunchMissionController instance;
    public TMP_Text missionSelText;
    SelectMissionButton[] buttonChildren;

    private void Awake()
    {
        instance = this;
        buttonChildren = FindObjectsOfType<SelectMissionButton>();
        missionSelText.text = "null array - please input";
    }

    public void SelectButton(SelectMissionButton button)
    {
        foreach(SelectMissionButton but in buttonChildren)
        {
            if(but != button)
                but.SetInactive();
        }
        button.SetActive();
        missionSelText.text = button.toolTip;
    }
}
