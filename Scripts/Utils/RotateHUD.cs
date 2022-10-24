using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates HUD elements to be always facing the player camera
/// </summary>
public class RotateHUD : MonoBehaviour
{
    float startX;
    float startY;
    float startZ;
    float startW;

    Transform cam;


    // Start is called before the first frame update
    void Start()
    {
        startX = transform.rotation.eulerAngles.x;
        startY = transform.rotation.eulerAngles.y;
        startZ = transform.rotation.eulerAngles.z;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = new Quaternion(startX, startY, startZ, transform.rotation.w);
        transform.LookAt(cam);
    }
}
