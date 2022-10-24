using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys an audiosource once it has stopped playing, to reduce memory clutter.
/// </summary>
public class DestroyAudioSource : MonoBehaviour
{
    float time;
    private void Awake()
    {
        time = GetComponent<AudioSource>().clip.length + Time.time;
    }

    void Update()
    {
        if(Time.time > time)
        {
            Destroy(gameObject);
        }
    }
}
