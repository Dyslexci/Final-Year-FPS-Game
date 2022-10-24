// Deprecated - overriden by the Sniper.cs script

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenericHitscan : MonoBehaviour
{
    public Transform targetSphere;

    GenericWeapon masterWep;
    GroundWeapon wep;
    public bool currentlyActive;

    public TMP_Text currentAmmoCounter;
    public TMP_Text maxAmmoCounter;

    // Start is called before the first frame update
    void Start()
    {
        masterWep = GetComponent<GenericWeapon>();
        wep = GetComponent<GroundWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentlyActive)
            return;

        RaycastHit hit;
        Vector3 cameraTransform = wep.inv.gameObject.GetComponent<FirstPersonMovementController>().cameraTrans;
        Vector3 cameraRot = wep.inv.gameObject.GetComponent<FirstPersonMovementController>().cameraRot;

        if (Physics.Raycast(cameraTransform, cameraRot, out hit, 696, ~LayerMask.GetMask("Player")))
        {
            targetSphere.gameObject.SetActive(true);
            targetSphere.position = hit.point;
            if (Input.GetButtonDown("Fire1") && wep.inv.currentWeapon.currentAmmo > 0 && !wep.inv.currentWeapon.CheckIfBusy())
            {
                if (wep.inv.currentWeapon.currentAmmo > 0)
                {
                    if (!wep.inv.currentWeapon.CheckIfBusy())
                    {
                        Shoot(cameraRot, cameraTransform);
                    }
                }
            }
        }
        else
        {
            targetSphere.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.R) && !wep.inv.currentWeapon.CheckIfBusy() && wep.inv.currentWeapon.currentAmmo < wep.inv.currentWeapon.maxAmmo)
        {
            wep.inv.currentWeaponPrefab.GetComponent<Animator>().SetTrigger("Reload");
            wep.inv.currentWeapon.Reload();
        }

        *//*        if(Input.GetKeyDown(KeyCode.I) && !inventory.currentWeapon.CheckIfBusy())
                {
                    if(gunUp)
                    {
                        gunUp = false;
                        inventory.currentWeaponPrefab.GetComponent<Animator>().SetTrigger("PutAwayGun");
                        inventory.currentWeapon.PutAWay();
                    } else
                    {
                        gunUp = true;
                        inventory.currentWeaponPrefab.GetComponent<Animator>().SetTrigger("PullUpGun");
                        inventory.currentWeapon.PullUp();
                    }
                }*//*

        if (wep.inv.currentWeapon == null)
        {
            currentAmmoCounter.text = "0";
            maxAmmoCounter.text = "0";
        }
        else
        {
            currentAmmoCounter.text = wep.inv.currentWeapon.currentAmmo.ToString();
        }
    }

    private void Shoot(Vector3 cameraRot, Vector3 cameraPos)
    {
        wep.inv.currentWeapon.Fire(cameraPos, cameraRot, wep.inv.currentWeaponPrefab.GetComponent<Animator>());
        wep.inv.currentWeaponPrefab.GetComponent<Animator>().SetTrigger("Fire");

    }
}
*/