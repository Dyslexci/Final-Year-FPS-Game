using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deprecated but leaving in in case of future use
/// </summary>
public class MenuElementController : MonoBehaviour
{
    public static IEnumerator FadeOut(CanvasGroup panel, float desiredAlpha, float rate)
    {
        while(panel.alpha > desiredAlpha)
        {
            panel.alpha -= rate;
            yield return new WaitForFixedUpdate();
        }
    }

    public static IEnumerator FadeIn(CanvasGroup panel, float desiredAlpha, float rate)
    {
        while (panel.alpha < desiredAlpha)
        {
            panel.alpha += rate;
            yield return new WaitForFixedUpdate();
        }
    }
}
