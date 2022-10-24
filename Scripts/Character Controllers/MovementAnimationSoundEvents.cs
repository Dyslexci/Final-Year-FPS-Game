using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimationSoundEvents : MonoBehaviour
{
    public static MovementAnimationSoundEvents instance;

    public AudioClip[] footSteps;
    public AudioClip[] runSteps;
    public AudioSource leftAudioSource;
    public AudioSource rightAudioSource;
    public AudioSource landSource;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Determines which footstep should play, and plays it. This method is called from an animation key attached to the walk and sprint animations.
    /// </summary>
    /// <param name="side"></param>
    public void Footstep(string side)
    {
        int randomSound = Random.Range(0, footSteps.Length);

        if(side.Equals("Left"))
        {
            leftAudioSource.clip = footSteps[randomSound];
            leftAudioSource.Play();
            return;
        }
        rightAudioSource.clip = footSteps[randomSound];
        rightAudioSource.Play();
    }

    /// <summary>
    /// Plays the running sound as keyed on the run animation.
    /// </summary>
    /// <param name="side"></param>
    public void Run(string side)
    {
        int randomSound = Random.Range(0, runSteps.Length);

        if (side.Equals("Left"))
        {
            leftAudioSource.clip = runSteps[randomSound];
            leftAudioSource.Play();
            return;
        }
        rightAudioSource.clip = runSteps[randomSound];
        rightAudioSource.Play();
    }

    /// <summary>
    /// Plays the jump sound when the player jumps.
    /// </summary>
    public void Jump()
    {
        int randomSound = Random.Range(0, footSteps.Length);
        leftAudioSource.clip = footSteps[randomSound];
        randomSound = Random.Range(0, footSteps.Length);
        rightAudioSource.clip = footSteps[randomSound];
        rightAudioSource.Play();
        leftAudioSource.Play();
    }
}
