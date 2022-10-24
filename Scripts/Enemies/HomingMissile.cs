using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 15.0f;
    public float rotateSpeed = 95.0f;
    public float deviationSpeed = 2.0f;
    public bool collided;
    public int damage;
    public LayerMask everythingExceptPlayer;
    public GameObject impactVFX;
    public GameObject hitSound;
    Rigidbody rb;
    Vector3 target;
    Vector3 standardPrediction, deviatedPrediction;

    float deviationAmount = 50;
    float range;
    public float damageRadius = 3.0f;
    bool wasPaused;
    Vector3 vel;
    GameObject marker;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Initialises targeting parameters
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    /// <param name="damage"></param>
    /// <param name="range"></param>
    /// <param name="marker"></param>
    internal void Initialise(Vector3 target, float speed, int damage, float range, GameObject marker)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
        this.range = range;
        this.marker = marker;
    }

    private void Update()
    {
        if (GameController.instance.paused && !wasPaused)
        {
            wasPaused = true;
            vel = rb.velocity;
            rb.velocity = Vector3.zero;
        }
        if (!GameController.instance.paused && wasPaused)
        {
            wasPaused = false;
            rb.velocity = vel;
        }
    }

    private void FixedUpdate()
    {
        if (GameController.instance.paused) return;

        var leadTimePercentage = Mathf.InverseLerp(5, range, Vector3.Distance(transform.position, target));

        rb.velocity = transform.forward * speed;
        AddDeviation(leadTimePercentage);
        Rotate();
    }

    /// <summary>
    /// Rotates the missile towards the target, used deviated predictions of where the target is.
    /// </summary>
    void Rotate()
    {
        var heading = deviatedPrediction - transform.position;
        var rotation = Quaternion.LookRotation(heading);
        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime));
    }

    /// <summary>
    /// Deviates slightly from the actual target position, so the missile wobbles in flight instead of being perfectly aimed.
    /// </summary>
    /// <param name="leadTimePercentage"></param>
    void AddDeviation(float leadTimePercentage)
    {
        var deviation = new Vector3(Mathf.Cos(Time.time * deviationSpeed), 0, 0);
        var predictionOffset = transform.TransformDirection(deviation) * deviationAmount * 0;

        deviatedPrediction = predictionOffset + target;
    }

    /// <summary>
    /// Detonates on impact with anything.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collided) return;

        Destroy(marker);
        if (collision.gameObject.CompareTag("Player"))
        {
            Instantiate(impactVFX, transform.position, transform.rotation);
            Instantiate(hitSound, transform.position, transform.rotation);
            collided = true;
            collision.gameObject.GetComponent<CharacterCombatController>().DamageHealth(damage);
            GetComponent<CinemachineImpulseSource>().GenerateImpulse(Vector3.one);
            Destroy(gameObject);
        }
        else
        {
            Instantiate(impactVFX, transform.position, transform.rotation);
            Instantiate(hitSound, transform.position, transform.rotation);
            //Instantiate(hitSound, transform.position, transform.rotation);
            collided = true;
            float distanceToPlayer = Vector3.Distance(transform.position, CharacterCombatController.instance.transform.position);
            if (distanceToPlayer < damageRadius)
            {
                float dmgMulti = Mathf.InverseLerp(0, damageRadius, distanceToPlayer);
                CharacterCombatController.instance.DamageHealth(Mathf.RoundToInt(damage * dmgMulti));
            }
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.DrawSphere(transform.position, damageRadius);
        if (target == null) return;

        

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(target, .15f);
    }
}
