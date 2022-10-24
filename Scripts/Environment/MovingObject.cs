using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public bool displayDebugGizmos = true;

    public Transform endPos;
    public float timeToArrive;
    public bool movementEnabled;
    public bool delayBetweenMovements;
    public float delay = 5.0f;
    public bool randomDelayVariation;
    public float randomDelayAmount = .2f;
    public Transform startPos;
    bool atEndPos;
    bool moving;
    float delayNewStartTime = 0;

    private void Awake()
    {
        //startPos = GetComponentInParent<Transform>().position;
        if (!delayBetweenMovements) randomDelayVariation = false;
    }

    /// <summary>
    /// Moves the object between the start and ending positions, with an optional delay. This class is designed to be as generic as possible to fulfil a variety of roles
    /// for e.g, moving cover, elevators.
    /// </summary>
    private void Update()
    {
        movementEnabled = !GameController.instance.paused;
        if (!movementEnabled)
        {
            return;
        }
        float newDelay = delay;
        if (delayBetweenMovements)
        {
            if (Time.time < delayNewStartTime) return;
            
            if (randomDelayVariation)
            {
                newDelay = delay * Random.Range(-randomDelayAmount, randomDelayAmount);
            }
        }


        if(!atEndPos)
        {
            MoveToEndPos();
            if (Vector3.Distance(gameObject.transform.position, endPos.position) < .05f)
            {
                atEndPos = true;
                moving = false;
                    
                
                delayNewStartTime = Time.time + newDelay;
            }
            return;
        }
        MoveToStartPos();
        if (Vector3.Distance(gameObject.transform.position, startPos.position) < .05f)
        {
            atEndPos = false;
            moving = false;
            delayNewStartTime = Time.time + newDelay;
        }
            
    }

    void MoveToEndPos()
    {
        if(!moving)
        {
            LeanTween.move(gameObject, endPos, timeToArrive).setEase(LeanTweenType.easeInOutQuad);
            moving = true;
        }
            
    }

    void MoveToStartPos()
    {
        if (!moving)
        {
            LeanTween.move(gameObject, startPos, timeToArrive).setEase(LeanTweenType.easeInOutQuad);
            moving = true;
        }
            
    }

    private void OnDrawGizmos()
    {
        if (!displayDebugGizmos) return;
        if (endPos == null) return;
        if (startPos == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(endPos.position, .5f);
        Gizmos.DrawSphere(startPos.position, .5f);
        Gizmos.DrawLine(startPos.position, endPos.position);
    }
}
