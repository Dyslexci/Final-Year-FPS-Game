using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Smasher : Enemy
{
    public bool displayDebugGizmos;
    public Transform testTransform;

    public float sightRange = 30;
    public float attackRange = 10;
    public float idleRange = 10;

    public float attackWindup = .5f;
    public float attackCooldown = 2.0f;

    public LayerMask groundMask, playerMask;

    Transform player;
    Vector3 originPoint, idleNavPoint;
    public bool remainStationary, idleSet, playerInRange, attackPathSet, attackOnCooldown, attacking;
    float timeUntilNewIdlePoint;
    float timeUntilNextAttack = 0.0f;

    private void Awake()
    {
        originPoint = transform.position;
        player = GameObject.Find("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.paused)
        {
            navAgent.enabled = false;
            return;
        }
        navAgent.enabled = true;

        if (remainStationary)
        {
            return;
        }

        if (Time.time > timeUntilNextAttack) attackOnCooldown = false;
        playerInRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        if (!hasSeenPlayer) hasSeenPlayer = CheckSeenPlayer();

        if (!hasSeenPlayer)
        {
            Idle();
            return;
        }
        if (!playerInRange && !attackOnCooldown)
        {
            NavigateToPlayer();
            return;
        }
        if (!attacking && !attackOnCooldown)
        {
            Attack();
            return;
        }

    }

    public GameObject bomb;
    public Transform spawnPos;
    public AudioSource servoAudio;

    /// <summary>
    /// Throws a bomb forward.
    /// </summary>
    protected override void Attack()
    {
        print("attacking");
        animator.SetTrigger("Attack");
        timeUntilNextAttack = Time.time + 3;
        attackOnCooldown = true;
        servoAudio.Play();
    }

    /// <summary>
    /// Generic idle behaviour, patrolling around the original spawn point.
    /// </summary>
    protected override void Idle()
    {
        if (Time.time < timeUntilNewIdlePoint) return;

        if (!idleSet)
        {
            SearchIdleWalkPoint();
            if (idleSet) navAgent.SetDestination(idleNavPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - idleNavPoint;

        if (distanceToWalkPoint.magnitude < 1)
        {
            idleSet = false;
            timeUntilNewIdlePoint = Time.time + Random.Range(1.0f, 3.0f);
        }
    }

    /// <summary>
    /// Finds a random point near the spawn point to navigate toward.
    /// </summary>
    private void SearchIdleWalkPoint()
    {
        float randomZ = Random.Range(-idleRange, idleRange);
        float randomX = Random.Range(-idleRange, idleRange);

        idleNavPoint = new Vector3(originPoint.x + randomX, originPoint.y, originPoint.z + randomZ);

        if (Physics.Raycast(idleNavPoint, -transform.up, 2f, groundMask))
        {
            idleSet = true;
        }
    }

    /// <summary>
    /// Navigates to a set distance away from the player.
    /// </summary>
    protected override void NavigateToPlayer()
    {
        Vector3 P2 = player.position;
        Vector3 P1 = transform.position;
        float circumferenceVariation = Random.Range(-20.0f, 20.0f);
        Vector3 P3 = P2 + Vector3.ClampMagnitude(Quaternion.AngleAxis(circumferenceVariation, Vector3.up) * (P1 - P2), attackRange);

        navAgent.SetDestination(P3);
    }

    /// <summary>
    /// Determines whether the player has been seen.
    /// </summary>
    /// <returns></returns>
    private bool CheckSeenPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        if (distanceToPlayer > sightRange) return false;

        if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, groundMask))
        {
            return true;
        }
        return false;
    }
}
