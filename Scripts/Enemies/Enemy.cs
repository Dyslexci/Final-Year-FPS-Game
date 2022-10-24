using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public GameObject explosionPrefab;
    public string enemyName;
    public float health = 100f;
    public int damage = 10;
    public int numberOfRespawns;
    public RoomController room;
    public Transform spawnPoint, respawnBin;
    public float respawnTime = 1;
    public bool hasSeenPlayer;
    public NavMeshAgent navAgent;
    public Animator animator;
    public MeshRenderer[] meshRenderer;
    float integrateRate = 0.02f;
    float refreshRate = 0.05f;
    public Material[] dissolveMaterials;
    public bool wasPaused;
    string lastDamageType = "";

    int timesRespawned = 0;
    public float maxHealth;
    bool dead;

    protected abstract void Idle();

    protected abstract void NavigateToPlayer();

    protected abstract void Attack();

    public void initialise(int _numberOfRespawns, RoomController _room, Transform _spawnPoint, Transform _respawnBin)
    {
        numberOfRespawns = _numberOfRespawns;
        room = _room;
        spawnPoint = _spawnPoint;
        respawnBin = _respawnBin;
        maxHealth = health;
    }

    /// <summary>
    /// Manages everything related to respawning the unit.
    /// </summary>
    public void Respawn()
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        //gameObject.SetActive(false);
        hasSeenPlayer = false;
        numberOfRespawns -= 1;
        navAgent.Warp(respawnBin.position);
        navAgent.isStopped = true;
        if(animator)
        {
            animator.SetBool("Dead", false);
        }
        StartCoroutine(RespawnDelay());
    }

    /// <summary>
    /// Delays respawning by an amount, de-activating the AI for this period and moving it between the play-grounds and the temporary container.
    /// </summary>
    /// <returns></returns>
    IEnumerator RespawnDelay()
    {
        health = maxHealth;
        yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
        navAgent.isStopped = false;
        if(dissolveMaterials.Length > 0)
        {
            foreach(Material mat in dissolveMaterials)
            {
                mat.SetFloat("DissolveAmount", 1f);
            }
        }
        try
        {
            navAgent.Warp(spawnPoint.position);
        }
        catch (System.NullReferenceException)
        {
            navAgent.Warp(RoomController.instance.rooms[1].spawnPoints[0].position);
            Debug.LogError("AI has failed to respawn at its own location - respawning at new random position");
            //throw;
        }
        
        navAgent.isStopped = true;
        StartCoroutine(Integrate());
    }

    /// <summary>
    /// Plays animations, sound effects and VFX for taking damage, while managing the death / respawn state. The DamageUtil class accepted as a parameter
    /// contains additional information, as the message event used to trigger this method can only take one parameter.
    /// </summary>
    /// <param name="util"></param>
    public void DamageHealth(DamageUtil util)
    {
        if(animator)
        {
            animator.ResetTrigger("Fire");
            animator.SetTrigger("Damaged");
        }
        float _damage = util.damage;
        hasSeenPlayer = true;


        if(util.type.Equals("primary") && lastDamageType.Equals("secondary"))
        {
            _damage *= CharacterStatisticsController.primaryMultiplierFromSecondary;
        } else if(util.type.Equals("secondary") && lastDamageType.Equals("primary"))
        {
            _damage *= CharacterStatisticsController.secondaryMultiplierFromPrimary;
        }

        lastDamageType = util.type;
        _damage = Mathf.RoundToInt(_damage);

        if (health > _damage)
        {
            health -= _damage;
            return;
        }
        if(animator)
        {
            animator.ResetTrigger("Damaged");
            animator.ResetTrigger("Fire");
            animator.SetBool("Dead", true);
        }
        if(numberOfRespawns <= 0)
        {
            if (dead) return;
            dead = true;
            DestroyEnemy();
            return;
        }
        Respawn();
        //Destroy(gameObject);
    }

    void DestroyEnemy()
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        room.RemoveEnemy(transform.position);
        Destroy(gameObject);
    }

    /// <summary>
    /// Plays the shader animation for the respawn of this unit.
    /// </summary>
    /// <returns></returns>
    IEnumerator Integrate()
    {
        yield return new WaitForSeconds(.2f);
        float counter = 1f;
        if(dissolveMaterials.Length > 0)
        {
            while(dissolveMaterials[0].GetFloat("DissolveAmount") > -.5f)
            {
                counter -= integrateRate;
                foreach (Material mat in dissolveMaterials)
                {
                    mat.SetFloat("DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
        navAgent.isStopped = false;
        navAgent.enabled = true;
    }
}
