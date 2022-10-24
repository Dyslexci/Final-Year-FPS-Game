using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public int maxAmmo = 5;
    public int currentAmmo;
    public float fireDelay = .217f;
    public float fireTime;
    public float reloadTime;
    public float putAwayTime;
    public string pistolName = "Pistol";
    Animator anim;
    AnimationClip clip;

    public bool firing;
    public bool reloading;
    public bool puttingAway;
    public bool active = true;

    private void Start()
    {
        currentAmmo = maxAmmo;
        anim = GetComponent<Animator>();
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            switch(clip.name)
            {
                case "metarig|Fire":
                    fireTime = clip.length;
                    break;
                case "metarig|Reload":
                    reloadTime = clip.length;
                    break;
                case "metarig|PutAway":
                    putAwayTime = clip.length;
                    break;
            }
        }
    }

    public bool CheckIfBusy()
    {
        if(firing || reloading || puttingAway || !active)
        {
            return true;
        }
        return false;
    }

    public void Fire()
    {
        currentAmmo--;
        firing = true;
        StartCoroutine(FireIE());
    }

    IEnumerator FireIE()
    {
        yield return new WaitForSeconds(fireTime);
        firing = false;
        if(currentAmmo == 0)
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
        active = false;
        StartCoroutine(PutAwayIE());
    }

    IEnumerator PutAwayIE()
    {
        puttingAway = true;
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
