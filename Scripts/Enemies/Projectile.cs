using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    bool collided;
    public int damage = 5;
    public LayerMask everythingExceptPlayer;
    public GameObject impactVFX;
    public GameObject hitSound;
    Rigidbody rb;
    bool wasPaused;
    Vector3 vel;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Moves the missile in a forward heading along its velocity vector.
    /// </summary>
    private void Update()
    {
        if (GameController.instance.paused && !wasPaused)
        {
            wasPaused = true;
            vel = rb.velocity;
            rb.velocity = Vector3.zero;
        }
        if(!GameController.instance.paused && wasPaused)
        {
            wasPaused = false;
            rb.velocity = vel;
        }
    }

    /// <summary>
    /// Detonates on impact.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collided) return;

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
            //Instantiate(hitSound, transform.position, transform.rotation);
            collided = true;
            Destroy(gameObject);
        }
    }
}
