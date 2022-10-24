// Deprecated script managing the ability to pick up and use multiple weapons.


/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterInventoryController : MonoBehaviour
{
    *//* Array slot attribution:
     * 1 - Primary weapon
     * 2 - Secondary weapon
     * 3 - Melee weapon (optional)
     * 4 - Special weapon (optional)
     * 5 - Special weapon (optional)
     * 6 - No weapon (optional)
     *//*
    public GenericWeapon primaryWeapon;
    public GameObject primaryPrefab;
    public GenericWeapon secondaryWeapon;
    public GameObject secondaryPrefab;
    public GenericWeapon meleeWeapon;
    public GameObject meleePrefab;
    public GenericWeapon grenade;
    public GameObject grenadePrefab;
    public GenericWeapon specialWeapon1;
    public GameObject specialPrefab1;
    public GenericWeapon specialWeapon2;
    public GameObject specialPrefab2;

    public TMP_Text weaponNameText;
    public TMP_Text pickupText;

    public GenericWeapon currentWeapon;
    public int currentSlot;
    public GameObject currentWeaponPrefab;

    public Transform weaponPos;

    CharacterShootingController shootController;
    FirstPersonMovementController controller;
    Transform cameraTransform;

    public bool gunUp = true;


    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<FirstPersonMovementController>();
        cameraTransform = Camera.main.transform;
        weaponNameText.text = "None";
        shootController = GetComponent<CharacterShootingController>();
        pickupText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Drop currently held weapon
        if (Input.GetKeyDown(KeyCode.G) && currentWeapon != null)
        {
            DropWeapon(currentWeapon, currentWeaponPrefab);
        }

        // Swap to the indicated slot
        if (Input.GetKeyDown(KeyCode.Alpha1) && primaryWeapon != null)
        {
            StartCoroutine(SwapWeapon(primaryWeapon, primaryPrefab, 1));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && secondaryWeapon != null)
        {
            StartCoroutine(SwapWeapon(secondaryWeapon, secondaryPrefab, 2));
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha3) && meleeWeapon != null)
        {
            StartCoroutine(SwapWeapon(grenade, grenadePrefab, 3));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && grenade != null)
        {
            StartCoroutine(SwapWeapon(meleeWeapon, meleePrefab, 4));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && specialWeapon1 != null)
        {
            StartCoroutine(SwapWeapon(specialWeapon1, specialPrefab1, 5));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) && specialWeapon2 != null)
        {
            StartCoroutine(SwapWeapon(specialWeapon2, specialPrefab2, 6));
        }

        // Check for weapons on the ground the player can choose to pick up by pressing the interact key, and associated functionality
        CheckForWeaponSwap();

        // Allow player to inspect / lower weapon
        if (Input.GetKeyDown(KeyCode.I))
        {
            InspectWeapon();
        }
    }

    private void CheckForWeaponSwap()
    {
        RaycastHit hit;
        Physics.Raycast(cameraTransform.position, controller.cameraRot, out hit, 2.5f, ~LayerMask.GetMask("Player"));
        float bestDist = 5;
        Collider bestCollider = null;

        Collider[] colliders = Physics.OverlapSphere(hit.point, 1);
        foreach (Collider _hit in colliders)
        {
            if (Vector3.Distance(_hit.transform.position, hit.point) < bestDist && _hit.gameObject.CompareTag("Pickup"))
            {
                bestDist = Vector3.Distance(_hit.transform.position, hit.point);
                bestCollider = _hit;
            }
        }

        if (bestCollider != null && bestCollider.GetComponent<GroundWeapon>().available)
        {
            ForceEquipWeapon(bestCollider);
        }
        else
        {
            pickupText.gameObject.SetActive(false);
        }
    }

    IEnumerator SwapWeapon(GenericWeapon wep, GameObject pref, int slot)
    {
        if (currentWeapon != null)
        {
            currentWeapon.puttingAway = true;
            currentWeaponPrefab.GetComponent<Animator>().SetTrigger("PutAwayGun");
            currentWeapon.PutAWay();

            while (currentWeapon.puttingAway)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        EquipWeapon(wep, pref, slot);
    }

    void EquipWeapon(GenericWeapon weapon, GameObject prefab, int slot)
    {
        if(currentWeaponPrefab != null)
            currentWeaponPrefab.SetActive(false);

        currentWeapon = weapon;
        currentWeaponPrefab = prefab;
        currentWeaponPrefab.SetActive(true);
        currentWeapon.GetComponent<GenericHitscan>().maxAmmoCounter.text = currentWeapon.maxAmmo.ToString();
        currentWeapon.GetComponent<GenericHitscan>().currentlyActive = true;
        currentSlot = slot;
        weaponNameText.text = currentWeapon.weaponName;
    }

    void ForceEquipWeapon(Collider col)
    {
        int num = col.GetComponent<GenericWeapon>().slotNum;
        GenericWeapon weaponToSwap = col.GetComponent<GenericWeapon>();
        pickupText.gameObject.SetActive(true);

        if (num == 1)
        {
            //PerformWeaponSwap(col.GetComponent<GenericWeapon>(), num);
            if(primaryPrefab != null)
            {
                pickupText.text = "Press <color=#ffeb04>E<color=#ffffff> to swap to <color=#ffeb04>" + weaponToSwap.weaponName;
                if(Input.GetKeyDown(KeyCode.E))
                {
                    DropWeapon(primaryWeapon, primaryPrefab);
                    weaponToSwap.GetComponent<GroundWeapon>().Equip(this);
                    EquipWeapon(primaryWeapon, primaryPrefab, 1);
                }
            } else
            {
                pickupText.text = "Press <color=#ffeb04>E<color=#ffffff> to pick up <color=#ffeb04>" + weaponToSwap.weaponName;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    weaponToSwap.GetComponent<GroundWeapon>().Equip(this);
                    EquipWeapon(primaryWeapon, primaryPrefab, 1);
                }
            }
        } else if(num == 2)
        {
            //PerformWeaponSwap(col.GetComponent<GenericWeapon>(), num);
            if (secondaryPrefab != null)
            {
                pickupText.text = "Press <color=#ffeb04>E<color=#ffffff> to swap to <color=#ffeb04>" + weaponToSwap.weaponName;
                if (Input.GetKeyDown(KeyCode.E))
                {

                }
            }
            else
            {
                pickupText.text = "Press <color=#ffeb04>E<color=#ffffff> to pick up <color=#ffeb04>" + weaponToSwap.weaponName;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    weaponToSwap.GetComponent<GroundWeapon>().Equip(this);
                    EquipWeapon(secondaryWeapon, secondaryPrefab, 2);
                }
            }
        } else if(num == 3)
        {
            Debug.LogError("Weapon slot 3 is not yet implemented - CharacterInventoryController, line 121");
        }
    }

    private void DropWeapon(GenericWeapon weapon, GameObject prefab)
    {
        weapon.GetComponent<GenericHitscan>().currentlyActive = false;
        Transform cameraTransform = Camera.main.transform;

        Vector3 spawn = (cameraTransform.forward) + cameraTransform.position;
        Vector3 force = (cameraTransform.forward * 250);
        if(weapon == currentWeapon)
        {
            switch (currentSlot)
            {
                case 1:
                    primaryWeapon = null;
                    break;
                case 2:
                    secondaryWeapon = null;
                    break;
            }

            currentWeapon = null;
        }
        weapon.transform.position = spawn;
        weapon.GetComponent<GroundWeapon>().ReEnable();
        weapon.GetComponent<Rigidbody>().AddForce(force);
        Destroy(prefab);
        
        weapon = null;
        weaponNameText.text = "None";
    }

    void InspectWeapon()
    {
        if (gunUp && !currentWeapon.CheckIfBusy())
        {
            gunUp = false;
            currentWeaponPrefab.GetComponent<Animator>().SetTrigger("PutAwayGun");
            currentWeapon.PutAWay();
        }
        else if (!gunUp)
        {
            gunUp = true;
            currentWeaponPrefab.GetComponent<Animator>().SetTrigger("PullUpGun");
            currentWeapon.PullUp();
        }
    }
}
*/