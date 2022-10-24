using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Removes a visual effect when it has stopped emitting particles, to reduce memory clutter.
/// </summary>
public class VisualEffectCleaner : MonoBehaviour
{
    VisualEffect vfx;
    float startTime;

    private void Awake()
    {
        vfx = GetComponent<VisualEffect>();
        startTime = Time.time;
    }

    private void LateUpdate()
    {
        if (vfx.aliveParticleCount <= 0 && Time.time > startTime + 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
