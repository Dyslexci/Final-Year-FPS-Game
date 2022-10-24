using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFade : MonoBehaviour
{
    public Color colour;
    public float speed = 10f;

    private LineRenderer lr;


    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        colour.a = Mathf.Lerp(colour.a, 0, Time.deltaTime * speed);

        lr.startColor = colour;
        lr.endColor = colour;
    }
}
