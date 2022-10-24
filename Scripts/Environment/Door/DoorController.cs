using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorController : MonoBehaviour
{
    public Transform entryPoint;
    public Transform exitPoint;
    public Animator animator;
    public bool active = false;
    public bool open = false;
    public bool autoClose = false;
    public Light light1, light2;
    public GameObject doorBlocker;

    public TMP_Text upgradeText;
    bool lastActive;
    bool autoCloseTriggered = false;

    public Material redLight;
    public Material greenLight;
    public GameObject doorMeshHolder;
    public Renderer doorRenderer;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        doorRenderer = doorMeshHolder.GetComponent<Renderer>();
        doorBlocker.SetActive(false);
/*        for(int i = 0; i < renderer.materials.Length; i++)
        {
            if(renderer.materials[i].name = )
            print("Found lighting");
        }*/
    }

    private void Update()
    {
        if (active == lastActive) return;
        lastActive = active;
        Material[] mats = doorRenderer.materials;
        if (active == false)
        {
            light1.color = Color.red;
            light2.color = Color.red;
            doorRenderer.materials[1].SetColor("_Color", Color.red * 4);
            return;
        }
        doorRenderer.materials[1].SetColor("_Color", Color.green * 4);
        light1.color = Color.green;
        light2.color = Color.green;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !autoCloseTriggered)
        {
            doorBlocker.SetActive(true);
            autoCloseTriggered = true;
            active = false;
            CloseDoor();
        }
    }

    public AudioSource openDoorSFX;
    public AudioSource closeDoorSFX;

    public void OpenDoor()
    {
        open = true;
        animator.SetBool("Open", true);
        print("Opening door");
        closeDoorSFX.Stop();
        openDoorSFX.Play();
    }

    public void CloseDoor()
    {
        open = false;
        animator.SetBool("Open", false);
        print("Closing door");
        openDoorSFX.Stop();
        closeDoorSFX.Play();
    }

    public void EnableLights()
    {
        light1.enabled = true;
        light2.enabled = true;
    }

    public void InitialiseUpgradeDisplay(string upgradeName)
    {
        upgradeText.text = upgradeName;
        upgradeText.gameObject.SetActive(true);
    }
}
