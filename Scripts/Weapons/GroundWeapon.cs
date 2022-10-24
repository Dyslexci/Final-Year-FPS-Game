// Deprecated - manages weapons on the ground

/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundWeapon : MonoBehaviour
{
    public GameObject prefab;
    public float pickupRadius = 5f;
    public Animator animator;

    public Transform depositPoint;

    GenericWeapon thisWeapon;
    public bool available = true;

    public CharacterInventoryController inv;

    private void Awake()
    {
        thisWeapon = GetComponent<GenericWeapon>();
        animator = GetComponent<Animator>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && available)
        {
            Debug.Log("Player has entered gun interactive zone");
            inv = other.gameObject.GetComponent<CharacterInventoryController>();

            Equip(inv);
        }
    }

    public void Equip(CharacterInventoryController _inv)
    {
        inv = _inv;
        if (thisWeapon.weaponType.Equals("Primary") && inv.primaryWeapon == null)
        {
            inv.primaryWeapon = thisWeapon;
            GameObject gun = AbstractedEquip(inv);
            inv.primaryPrefab = gun;
        }
        else if (thisWeapon.weaponType.Equals("Secondary") && inv.secondaryWeapon == null)
        {
            inv.secondaryWeapon = thisWeapon;
            GameObject gun = AbstractedEquip(inv);
            inv.secondaryPrefab = gun;
        }
        else if (thisWeapon.weaponType.Equals("Melee") && inv.meleeWeapon == null)
        {
            inv.meleeWeapon = thisWeapon;
            GameObject gun = AbstractedEquip(inv);
            inv.meleePrefab = gun;
        }
        else if (thisWeapon.weaponType.Equals("Grenade") && inv.grenade == null)
        {
            inv.grenade = thisWeapon;
            GameObject gun = AbstractedEquip(inv);
            inv.grenadePrefab = gun;
        }
        else if (thisWeapon.weaponType.Equals("Special1") && inv.specialWeapon1 == null)
        {
            inv.specialWeapon1 = thisWeapon;
            GameObject gun = AbstractedEquip(inv);
            inv.specialPrefab1 = gun;
        }
        else if (thisWeapon.weaponType.Equals("Special2") && inv.specialWeapon2 == null)
        {
            inv.specialWeapon2 = thisWeapon;
            GameObject gun = AbstractedEquip(inv);
            inv.specialPrefab2 = gun;
        }
    }

    GameObject AbstractedEquip(CharacterInventoryController inv)
    {
        GameObject gun = Instantiate(prefab, inv.weaponPos.position, inv.weaponPos.rotation, inv.weaponPos) as GameObject;
        gun.SetActive(false);
        Debug.Log("Picked up " + thisWeapon.weaponName);
        transform.position = depositPoint.position;
        GetComponent<Rigidbody>().isKinematic = true;
        available = false;
        return gun;
    }

    public void ReEnable()
    {
        inv = null;
        GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(Enable());
    }

    IEnumerator Enable()
    {
        yield return new WaitForSecondsRealtime(.5f);
        available = true;
    }
}
*/