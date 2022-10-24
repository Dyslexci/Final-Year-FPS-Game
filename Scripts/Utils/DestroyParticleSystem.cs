using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys a particle system / VFX object once it has stopped emitting particles, to reduce memory clutter.
/// </summary>
public class DestroyParticleSystem : MonoBehaviour
{
    private ParticleSystem thisObject;

    void Start()
    {
        thisObject = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(!thisObject.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
