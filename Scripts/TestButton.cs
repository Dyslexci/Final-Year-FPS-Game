using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test light - in playground
/// </summary>
public class TestButton : Interactible
{
    public Light testLight;
    bool green = true;

    private void Start()
    {
        testLight.color = Color.green;
    }

    public override void Interact()
    {
        if(green)
        {
            testLight.color = Color.red;
            green = false;
            return;
        }

        testLight.color = Color.green;
        green = true;

    }
}
