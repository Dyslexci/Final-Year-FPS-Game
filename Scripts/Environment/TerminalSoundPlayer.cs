using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class plays random terminal beeps and boops.
/// </summary>
public class TerminalSoundPlayer : MonoBehaviour
{
    public AudioClip[] sounds;
    AudioSource soundSource;
    float timeToPlayNextSound = 0;

    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timeToPlayNextSound)
        {
            PlaySound();
        }
    }

    /// <summary>
    /// Plays sound and chooses a random delay to play the next sound.
    /// </summary>
    void PlaySound()
    {
        AudioClip clip = sounds[Random.Range(0, sounds.Length)];
        soundSource.clip = clip;
        soundSource.Play();
        timeToPlayNextSound = Time.time + clip.length + Random.Range(.5f, 4f);
    }
}
