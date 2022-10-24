using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class managing the noises keyframed by the Caterpillar attack animation.
/// </summary>
public class CaterBotAttack : MonoBehaviour
{
    public Transform spawnPos;
    public GameObject bomb;
    public AudioSource shotSound;
    public AudioSource servo1;
    public AudioSource servo2;
    public AudioSource servo1retract;
    public AudioSource servo2retract;

    public void Attack()
    {
        GameObject droppedBomb = Instantiate(bomb, spawnPos.position, spawnPos.rotation);
        droppedBomb.GetComponent<Rigidbody>().velocity = spawnPos.forward * 4;
        shotSound.Play();
    }

    public void PlayServo1()
    {
        servo1.Play();
    }

    public void PlayServo2()
    {
        servo2.Play();
    }

    public void PlayServo1Retract()
    {
        servo1retract.Play();
    }

    public void PlayServo2Retract()
    {
        servo2retract.Play();
    }
}
