using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Reveals chars in a text box one by one, to give the effect of teletyping.
/// </summary>
public class TeleType : MonoBehaviour
{
    public int chars;
    TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        text.maxVisibleCharacters = 0;
        chars = text.textInfo.characterCount;
    }

    public void Init()
    {
        StartCoroutine(RunTeleType());
    }

    IEnumerator RunTeleType()
    {
        int totalVisChars = text.textInfo.characterCount;
        int counter = 0;

        while(true)
        {
            int visCount = counter % (totalVisChars + 1);
            text.maxVisibleCharacters = visCount;

            if (visCount >= totalVisChars) break;

            counter += 1;
            yield return new WaitForSeconds(.05f);
        }
    }
}
