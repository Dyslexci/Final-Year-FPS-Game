using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages displaying and removing elements from the tutorial on playing the game visible at the start of each room.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public GameObject tutorialContainer;
    public GameObject[] tutorialTips;
    bool tut1;
    bool tut2;
    bool tut3;
    bool tut4;
    bool tutorialComplete;

    private void Awake()
    {
        instance = this;
        tutorialContainer.SetActive(false);
    }

    private void Update()
    {
        if (tutorialComplete) return;
        if(tut1 && tut2 && tut3 && tut4)
        {
            tutorialComplete = true;
            StartCoroutine(CloseTutorial());
        }
    }

    IEnumerator OpenTutorial()
    {
        tut1 = tut2 = tut3 = tut4 = false;
        tutorialContainer.GetComponent<CanvasGroup>().alpha = 0;
        tutorialContainer.SetActive(true);
        tutorialContainer.GetComponent<CanvasGroup>().LeanAlpha(1, .5f);
        yield return new WaitForSeconds(.5f);
    }

    IEnumerator CloseTutorial()
    {
        tut1 = tut2 = tut3 = tut4 = false;
        tutorialContainer.GetComponent<CanvasGroup>().LeanAlpha(0, 1);
        yield return new WaitForSeconds(1);
        tutorialContainer.SetActive(false);
        tutorialContainer.GetComponent<CanvasGroup>().alpha = 1;
    }

    public void DisplayTutorial()
    {
        StartCoroutine(OpenTutorial());
    }

    public void FinishTut1()
    {
        tut1 = true;
        tutorialTips[0].GetComponent<CanvasGroup>().LeanAlpha(.3f, .25f);
    }

    public void FinishTut2()
    {
        tut2 = true;
        tutorialTips[1].GetComponent<CanvasGroup>().LeanAlpha(.3f, .25f);
    }

    public void FinishTut3()
    {
        tut3 = true;
        tutorialTips[2].GetComponent<CanvasGroup>().LeanAlpha(.3f, .25f);
    }

    public void FinishTut4()
    {
        tut4 = true;
        tutorialTips[3].GetComponent<CanvasGroup>().LeanAlpha(.3f, .25f);
    }
}
