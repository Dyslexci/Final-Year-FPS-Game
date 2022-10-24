using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmerCrab : Enemy
{
    public bool displayDebugGizmos;
    public Transform testTransform;

    public float sightRange = 30;
    public float attackRange = 10;
    public float idleRange = 10;

    public float attackWindup = .5f;
    public float attackCooldown = 2.0f;

    public float idleSpeed = 3;
    public float normalSpeed = 10;
    public float attackSpeed = 30;

    public LayerMask groundMask, playerMask;

    Transform player, mainCam;
    Vector3 originPoint, idleNavPoint, attackPoint;
    CharacterCombatController playerCombatController;
    public bool remainStationary, idleSet, playerInRange, attackPathSet, attackOnCooldown, attacking;
    float timeUntilNewIdlePoint, attackWindupTime;
    float timeUntilNextAttack = 0.0f;
    float timeUntilNextDamage = 0.0f;
    float damageCooldown = 1.0f;

    private void Awake()
    {
        originPoint = transform.position;
        player = GameObject.Find("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
        playerCombatController = player.GetComponent<CharacterCombatController>();
        mainCam = Camera.main.gameObject.transform;
        navAgent.speed = idleSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.paused)
        {
            navAgent.enabled = false;
            return;
        }
        

        if (remainStationary)
        {
            navAgent.enabled = false;
            return;
        }

        navAgent.enabled = true;

        CheckPlayerContact();

        if (Time.time > timeUntilNextAttack) attackOnCooldown = false;
        playerInRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        if (!hasSeenPlayer) hasSeenPlayer = CheckSeenPlayer();

        if (!hasSeenPlayer)
        {
            Idle();
            return;
        }
        if (!playerInRange && !attackPathSet)
        {
            NavigateToPlayer();
            return;
        }
        if (attackOnCooldown)
        {
            Idle();
            return;
        }
        if(!attacking)
        {
            MoveToPlayer();
            return;
        }
        if(!attackOnCooldown)
        {
            MoveToPlayer();
        }
        
    }

    protected override void Attack()
    {
        attacking = true;
        attackPoint = CreateAttackPath();
        attackWindupTime = Time.time + attackWindup;
        remainStationary = true;

        navAgent.isStopped = true;
        navAgent.updatePosition = false;

        StartCoroutine(AttackWindup());
    }
    
    /// <summary>
    /// Moves the unit on a created vector of attack, passing through the original point of the player.
    /// </summary>
    void MoveOnAttackVector()
    {
        if (!attackPathSet) attacking = false;

        float step = attackSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, attackPoint, step);

        if (Vector3.Distance(transform.position, attackPoint) < 1.0f)
        {
            attackPathSet = false;
            attackOnCooldown = true;
            remainStationary = true;
            attackWindupTime = Time.time + attackWindup;
            timeUntilNextAttack = Time.time + attackCooldown;

            StartCoroutine(AttackWinddown());
            
        }
    }

    void MoveToPlayer()
    {
        navAgent.speed = attackSpeed;
        navAgent.SetDestination(player.position);
    }

    IEnumerator AttackWindup()
    {
        yield return new WaitForSeconds(attackWindup);
        remainStationary = true;
    }

    IEnumerator AttackWinddown()
    {
        yield return new WaitForSeconds(attackWindup);
        navAgent.isStopped = false;
        navAgent.Warp(transform.position);
        navAgent.updatePosition = true;

        remainStationary = false;
    }

    /// <summary>
    /// Creates a new position on the opposite side of the player.
    /// </summary>
    /// <returns></returns>
    Vector3 CreateAttackPath()
    {
        Vector3 P2 = player.position;
        Vector3 P1 = transform.position;

        Vector3 P3 = P2 + Vector3.ClampMagnitude(Quaternion.AngleAxis(180, Vector3.up) * (P1 - P2), attackRange);
        Vector3 P3y = P2 + Vector3.ClampMagnitude(Quaternion.AngleAxis(180, Vector3.right) * (P1 - P2), attackRange * 2);
        //P3.y = P3y.y;
        attackPathSet = true;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, P3 - P1, out hit, groundMask))
        {
            P3 = Vector3.MoveTowards(hit.point, transform.position, navAgent.radius);
        }

        return P3;
    }

    /// <summary>
    /// Checks whether the player has been hit, and deals damage.
    /// </summary>
    void CheckPlayerContact()
    {
        if (Time.time < timeUntilNextDamage) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position) - 1f;
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        if (distanceToPlayer > 0.15f) return;

        if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, groundMask))
        {
            print("damaged");
            timeUntilNextDamage = Time.time + damageCooldown;
            playerCombatController.DamageHealth(10);
        }

            //if (distanceToPlayer > navAgent.radius + .5f + .15f) return;
        
    }

    /// <summary>
    /// Generic idle behaviour, patrolling around the original spawn point.
    /// </summary>
    protected override void Idle()
    {
        navAgent.speed = idleSpeed;
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

    private void SearchIdleWalkPoint()
    {
        float randomZ = Random.Range(-idleRange, idleRange);
        float randomX = Random.Range(-idleRange, idleRange);

        idleNavPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

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
        navAgent.speed = normalSpeed;
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
            navAgent.speed = normalSpeed;
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (!displayDebugGizmos) return;

        Vector3 P2 = testTransform.position;
        Vector3 P1 = transform.position;

        //Vector3 P3 = P2 + Vector3.ClampMagnitude(Quaternion.AngleAxis(180, Vector3.up) * (P1 - P2), attackRange * 2);
        Vector3 P3 = P2 + Vector3.ClampMagnitude(Quaternion.AngleAxis(180, Vector3.up) * (P1 - P2), attackRange * 2);
        Vector3 P3y = P2 + Vector3.ClampMagnitude(Quaternion.AngleAxis(180, Vector3.right) * (P1 - P2), attackRange * 2);
        P3.y = P3y.y;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, P3 - P1, out hit, groundMask))
        {
            P3 = Vector3.MoveTowards(hit.point, transform.position, .35f);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(P3, .35f);
        Gizmos.DrawLine(transform.position, P3);
    }
}
