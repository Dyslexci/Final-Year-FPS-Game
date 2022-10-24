using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericWeapon : MonoBehaviour
{
    [Header("Weapon Setup Variables")]
    public string weaponName = "Glock-17";
    public string weaponType = "Pistol";
    public int maxAmmo = 13;
    public float explosionRadius;
    public float explosionPower;
    public GameObject bulletHitPrefab;
    public GameObject FPSViewPrefab;
    public GameObject bulletSoundPrefab;
    public AudioSource bulletHitSound;

    [Header("Internal Public Variables")]
    public int currentAmmo;
    public Animator anim;
    public int slotNum;
    //float fireDelay = .217f;
    public float fireTime;
    public float reloadTime;
    public float putAwayTime;

    bool firing;
    bool reloading;
    public bool puttingAway;
    bool active = true;
    

    private void Awake()
    {
        bulletHitSound = Instantiate(bulletSoundPrefab, this.transform.position, bulletHitPrefab.transform.rotation).GetComponent<AudioSource>();

        currentAmmo = maxAmmo;

        switch (weaponType)
        {
            case "Primary":
                slotNum = 1;
                break;
            case "Secondary":
                slotNum = 2;
                break;
            case "Grenade":
                slotNum = 3;
                break;
        }
    }

    public bool CheckIfBusy()
    {
        if (firing || reloading || puttingAway || !active)
        {
            return true;
        }
        return false;
    }

    public void Fire(Vector3 cameraPos, Vector3 cameraRot, Animator animator)
    {
        currentAmmo--;
        firing = true;

        RaycastHit hit;
        Physics.Raycast(cameraPos, cameraRot, out hit, 696, ~LayerMask.GetMask("Player"));

        Instantiate(bulletHitPrefab, hit.point, bulletHitPrefab.transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(hit.point, explosionRadius);
        animator.SetInteger("CurrentAmmo", currentAmmo);
        foreach (Collider _hit in colliders)
        {
            Rigidbody rb = _hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionPower, hit.point, explosionRadius, 25f);
            }
        }
        bulletHitSound.transform.position = hit.point;
        bulletHitSound.Play(0);

        StartCoroutine(FireIE());
    }

    IEnumerator FireIE()
    {
        yield return new WaitForSeconds(fireTime);
        firing = false;
        if (currentAmmo == 0)
        {
            Reload();
        }
    }

    public void Reload()
    {
        StartCoroutine(ReloadIE());
    }

    IEnumerator ReloadIE()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
        currentAmmo = maxAmmo;
    }

    public void PutAWay()
    {
        puttingAway = true;
        active = false;
        StartCoroutine(PutAwayIE());
    }

    IEnumerator PutAwayIE()
    {
        yield return new WaitForSeconds(putAwayTime);
        puttingAway = false;
    }

    public void PullUp()
    {
        StartCoroutine(PullUpIE());
    }

    IEnumerator PullUpIE()
    {
        puttingAway = true;
        yield return new WaitForSeconds(putAwayTime);
        puttingAway = false;
        active = true;
    }
}
