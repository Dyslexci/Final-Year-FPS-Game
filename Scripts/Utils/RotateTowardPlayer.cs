using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates the object this class is attached to so it always faces directly at the player camera.
/// </summary>
public class RotateTowardPlayer : MonoBehaviour
{
    public Transform _camera;
    public float radsPerSecond = 1.0f;

    private void Awake()
    {
        if(_camera == null)
            _camera = GameObject.Find("Player").transform;
    }

    void Update()
    {
        Vector3 targetDir = _camera.position - transform.position;
        float singleStep = radsPerSecond * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
