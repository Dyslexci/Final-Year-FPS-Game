using CharacterScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.VFX;
using Michsky.UI.ModernUIPack;
using Random = UnityEngine.Random;
using Cinemachine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

public class Sniper : MonoBehaviour
{
    [Header("GUN STATS")]
    [Tooltip("The damage the gun deals")]
    public float critDamage = 70f;
    [Tooltip("The time to hit the critical damage period in seconds")]
    public float initialCharge = 1.2f;
    [Tooltip("The duration in which a critical shot can be fired in seconds")]
    public float criticalDuration = .3f;
    [Tooltip("The minimum damage the weapon can deal, assuming they simply click the primary fire key")]
    public float minDamage = 20f;
    [Tooltip("The maximum damage the weapon can deal")]
    public float maxDamage = 60f;
    [Tooltip("Multiplier for critical damage")]
    public float critDamageMultiplier = 1.2f;
    [Tooltip("The range of the primary attack")]
    public float primaryAttackRange = 40;
    [Tooltip("Point from which bullets/trails originate")]
    public Transform bulletOriginPoint;
    public float primaryShotCooldown = .7f;
    public float secondaryCooldown = 1.0f;

    public Animator animator;

    [Header("PREFABS")]
    [Tooltip("Prefab for projectile")]
    public GameObject bulletPrefab;

    public GameObject bulletImpact;

    public GameObject bulletHitParticles;
    [Tooltip("Prefab for bullet trail")]
    public GameObject bulletTrailPrefab;
    [Tooltip("VFX tied to the bullet trail")]
    public VisualEffect bulletVFX;
    [Tooltip("VFX for the muzzle flash")]
    public VisualEffect muzzleFlash;

    [Header("HUD ELEMENTS")]
    [Tooltip("The bar indicating gun charge")]
    public ProgressBar chargeIndicator;
    [Tooltip("The indicator for a critical hit")]
    public Image critIndicator;
    [Tooltip("Crosshair image")]
    public Image crosshair;

    public AudioSource audioSource;
    public AudioClip chargeUp;
    public AudioClip loopCharge;
    public AudioClip[] firePrimary;
    public AudioClip cancelFire;
    public AudioClip[] fireSecondary;
    public AudioSource[] foley;
    public AudioSource reload;

    // private logic
    private bool charging;
    private bool critical;
    private float timeHeld;
    private float percentHeld;
    private Coroutine lastChargeRoutine = null;
    private Coroutine lastCritRoutine = null;
    Coroutine chargeSountRoutine = null;
    private Vector3 lasthit;
    float timeToNextPrimary = 0.0f;
    float timeToNextSecondary = 0.0f;
    float cancelShotTimer = 0.0f;

    // general necessary components
    private PlayerInputs _input;
    private Camera _mainCamera;

    Animator anim;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    private void Start()
    {
        _input = GetComponentInParent<PlayerInputs>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameController.instance.paused) return;

        Fire();
        SpecialAttack();
        UpdateCrosshair();
    }

    /// <summary>
    /// Allows the player to hold down the primary fire key to begin charging a shot, and fire that shot upon releasing the key. The shot fired will have damage dependant on how long the weapon
    /// was charged, and allows a critical shot.
    /// </summary>
    private void Fire()
    {
        if (Time.time <= timeToNextPrimary) return;

        if (_input.primaryfire)
        {
            if (!charging)
            {
                cancelShotTimer = Time.time + .2f;
                lastChargeRoutine = StartCoroutine(ChargeUp());
                chargeSountRoutine = StartCoroutine(ChargeSound());
                charging = !charging;
            }
        } else
        {
            if (charging)
            {
                StopCoroutine(chargeSountRoutine);
                audioSource.Stop();
                audioSource.loop = false;

                if (Time.time <= cancelShotTimer)
                {
                    animator.SetTrigger("CancelShot");
                    audioSource.clip = cancelFire;
                    audioSource.Play();
                    return;
                }

                int randomNum = Random.Range(0, firePrimary.Length);
                audioSource.clip = firePrimary[randomNum];
                audioSource.Play();

                // everything here are actions to be taken when the gun actually fires (i.e. the player releases the fire key)
                timeToNextPrimary = Time.time + primaryShotCooldown;
                StopCoroutine(lastChargeRoutine);

                animator.SetBool("HoldingCharge", false);

                float tempDamage = Mathf.Lerp(CharacterStatisticsController.primaryMinDamage, CharacterStatisticsController.primaryMaxDamage, percentHeld);
                float tempCritDamage = CharacterStatisticsController.primaryCritDamage;
                if (UnityEngine.Random.Range(0, 100) < CharacterStatisticsController.critChance)
                {
                    tempDamage = tempDamage * CharacterStatisticsController.critMultiplier;
                    critDamage = CharacterStatisticsController.primaryCritDamage * CharacterStatisticsController.critMultiplier;
                }
                TutorialManager.instance.FinishTut1();
                if (critical)
                {
                    TutorialManager.instance.FinishTut3();
                    critIndicator.gameObject.SetActive(false);
                    critical = false;
                    StopCoroutine(lastCritRoutine);
                    Shot(critDamage, percentHeld, "primary");
                } else
                {
                    Shot(tempDamage, percentHeld, "primary");
                }
                critDamage = tempCritDamage;
                timeHeld = 0f;
                percentHeld = 0f;
                charging = !charging;
                chargeIndicator.currentPercent = 0;
            }
        }
    }

    [Header("SPECIAL ATTACK")]
    [Tooltip("The total angle the special attack covers (plus and minus half this value)")]
    public float specialAttackAngle = 30;
    public int specialAttackNumberOfShots = 4;
    public float specialAttackRange = 40;
    public float specialAttackDamage = 10;
    public float specialAttackBulletSpeed = 40;

    /// <summary>
    /// This function must fire a volley of shots in a horizontal, equal spread, and allow correct scaling with modifiers such as additional shots in the volley, tighter volley angle and etc.
    /// </summary>
    private void SpecialAttack()
    {
        if (charging) return;

        if (Time.time <= timeToNextSecondary) return;

        if(_input.secondary)
        {
            TutorialManager.instance.FinishTut2();
            audioSource.loop = false;
            int randomNum = Random.Range(0, fireSecondary.Length);
            audioSource.clip = fireSecondary[randomNum];
            audioSource.Play();
            reload.Play();

            timeToNextSecondary = Time.time + secondaryCooldown;
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("Landing");
            animator.SetTrigger("FireSpecial");
            // calculate the starting angle for the first shot in the volley, from the left
            float angleLeft = -(CharacterStatisticsController.secondaryAttackAngle / 2);
            Quaternion firstAngleRotation = Quaternion.Euler(0, angleLeft, 0);
            float incidentAngles = CharacterStatisticsController.secondaryAttackAngle / (CharacterStatisticsController.secondaryNumberOfShots - 1);

            Quaternion[] angles = new Quaternion[CharacterStatisticsController.secondaryNumberOfShots];

            for (int i = 0; i < CharacterStatisticsController.secondaryNumberOfShots; i++)
            {
                float tempAngle = incidentAngles * i;
                tempAngle = angleLeft + tempAngle;
                angles[i] = Quaternion.Euler(0, tempAngle, 0);
            }

            Vector3[] hitPoints = new Vector3[CharacterStatisticsController.secondaryNumberOfShots];
            for (int i = 0; i < CharacterStatisticsController.secondaryNumberOfShots; i++)
            {
                Vector3 newRot = angles[i] * _mainCamera.transform.forward;
                Ray ray = new Ray(_mainCamera.transform.position, newRot);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, specialAttackRange, ~LayerMask.GetMask("Player")))
                {
                    hitPoints[i] = hit.point;
                    if (hit.transform.gameObject.CompareTag("Enemy"))
                        hit.collider.gameObject.SendMessage("DamageHealth", new DamageUtil((int)CharacterStatisticsController.secondaryDamage, "secondary"), SendMessageOptions.DontRequireReceiver);
                    if (hit.transform.gameObject.CompareTag("Shield"))
                    {
                        hit.collider.gameObject.GetComponent<Shield>().Damage((int)CharacterStatisticsController.secondaryDamage);
                        lasthit = hit.point;
                        Instantiate(bulletImpact, hit.point, transform.rotation);
                    }
                }
                else
                {
                    hitPoints[i] = ray.GetPoint(specialAttackRange);
                }
            }

            foreach (Vector3 hit in hitPoints)
            {
                SpawnBulletTrail(hit, 1f);

            }

            _input.secondary = false;
        }
    }

    /// <summary>
    /// Performs the shot actions - raycasting out from the camera (important!), and allowing penetration through multiple specified layers, while stopping penetration on contact with other layers
    /// This penetration should be used to allow multiple enemies to be hit, as well as allowing the shot to ignore random decorative clutter which may get in the way of combat
    /// </summary>
    private void Shot(float damage, float shotPower, string type)
    {
        damage = Mathf.RoundToInt(damage);
        RaycastHit[] hits;
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        hits = Physics.RaycastAll(ray, primaryAttackRange, ~LayerMask.GetMask("Player"));
        GetComponent<CinemachineImpulseSource>().GenerateImpulse(Vector3.one * Mathf.Clamp(shotPower, 0, 0.95f));

        if (hits.Length > 0)
        {
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.CompareTag("Enemy"))
                {
                    hit.collider.gameObject.SendMessage("DamageHealth", new DamageUtil((int)damage, type), SendMessageOptions.DontRequireReceiver);
                    lasthit = hit.point;
                    Instantiate(bulletImpact, hit.point, transform.rotation);
                }
                else if(hit.transform.gameObject.CompareTag("Shield"))
                {
                    hit.collider.gameObject.GetComponent<Shield>().Damage((int)damage);
                    lasthit = hit.point;
                    Instantiate(bulletImpact, hit.point, transform.rotation);
                    break;
                } else
                {
                    lasthit = hit.point;
                    Instantiate(bulletImpact, hit.point, transform.rotation);
                    break;
                }
                
            }
            SpawnBulletTrail(lasthit, shotPower);
            return;
        }
        SpawnBulletTrail(null, shotPower);
    }

    /// <summary>
    /// Spawns a simple 2D line in the 3D worldspace between the barrel of the gun and the position of the last impact of the bullet
    /// </summary>
    /// <param name="hitPoint"></param>
    private void SpawnBulletTrail(Vector3? hitPoint, float? shotPower)
    {
        muzzleFlash.Play();
        if (hitPoint == null)
        {
            hitPoint = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 40));
        }

        GameObject bulletTrailEffect = Instantiate(bulletTrailPrefab, bulletOriginPoint.position, Quaternion.identity);
        Vector3 direction = ((Vector3)hitPoint - bulletOriginPoint.position).normalized;
        bulletTrailEffect.transform.rotation = Quaternion.LookRotation(direction);
        bulletVFX = bulletTrailEffect.GetComponent<VisualEffect>();

        float distance = Vector3.Distance(bulletOriginPoint.position, (Vector3)hitPoint);
        bulletVFX.SetFloat("Length", distance);
    }

    /// <summary>
    /// Increases the percentage of full charge the player has reached while holding down the primary fire key. This allows a shot to be prematurely fired for lower damage, as well as triggering
    /// an enhanced critical shot when reaching full charge. The charge starts at a minimum damage value to ensure that even spamming shots will still deal largely reduced damage, as opposed to 0.
    /// </summary>
    /// <returns></returns>
    IEnumerator ChargeUp()
    {
        animator.SetBool("HoldingCharge", true);
        while(timeHeld < initialCharge)
        {
            timeHeld += Time.deltaTime;
            percentHeld = timeHeld / initialCharge;
            double percentCharged = Math.Round(percentHeld, 2);
            chargeIndicator.currentPercent = percentHeld * 100;
            yield return new WaitForEndOfFrame();
        }
        percentHeld = 100f;
        chargeIndicator.currentPercent = 100;
        critical = true;
        lastCritRoutine = StartCoroutine(CriticalPeriod());
    }

    /// <summary>
    /// Enables a short critical period, where the player can release the primary fire key to fire an enhanced critical shot. After this period is over, if the player has still not released the key, a
    /// normal shot at 100% charge will be fired.
    /// </summary>
    /// <returns></returns>
    IEnumerator CriticalPeriod()
    {
        critIndicator.gameObject.SetActive(true);
        float timeDelta = 0f;
        while(timeDelta < CharacterStatisticsController.critDuration)
        {
            timeDelta += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        critical = false;
        critIndicator.gameObject.SetActive(false);
    }

    IEnumerator ChargeSound()
    {
        audioSource.loop = false;
        audioSource.clip = chargeUp;
        audioSource.Play();
        yield return new WaitForSeconds(chargeUp.length);
        audioSource.clip = loopCharge;
        audioSource.loop = true;
        audioSource.Play();
    }

    /// <summary>
    /// Updates the crosshair colour if the player is looking at an enemy. Also triggers the enemy health bar display, and informs the movement controller that
    /// the player is looking at an enemy.
    /// </summary>
    void UpdateCrosshair()
    {
        RaycastHit hit;
        if(Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Enemy")))
        {
            crosshair.color = Color.red;
            FirstPersonMovementController.instance.lookingAtEnemy = true;
            if(hit.transform.gameObject.CompareTag("Enemy"))
            {
                Enemy hitEnemy = hit.transform.GetComponent<Enemy>();
                UIController.instance.UpdateEnemyHealthbar((int)hitEnemy.health, (int)hitEnemy.maxHealth, hitEnemy.enemyName);
            }
            return;
        }
        FirstPersonMovementController.instance.lookingAtEnemy = false;
        UIController.instance.UpdateEnemyHealthbar(0, 0, "");
        crosshair.color = Color.white;
    }
}
