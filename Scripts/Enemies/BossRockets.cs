using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossRockets : Enemy
{
    public bool displayDebugGizmos;

    [Header("DEBUG TOGGLES")]
    [Tooltip("Toggle visual indicator of the range of attack")]
    public bool displayAttackRange;
    [Tooltip("Toggle visual indicator of the range of idle roam")]
    public bool displayRoamRange;
    [Tooltip("Toggle visual indicators for attack vectors and positioning")]
    //public bool displayAttackDebugInfo;
    public bool displayMovementInfo;
    [Tooltip("Transform component used to simulate player position while not in play mode")]
    public Transform centralPos;

    [Header("PUBLIC VALUES")]
    [Tooltip("Range in units the AI can see around itself")]
    public float sightRange = 30.0f;
    [Tooltip("Range in units from which the AI will begin an attack")]
    public float attackRange = 10.0f;

    public float dangerRange;
    [Tooltip("Range in units around its start position where the AI will randomly roam while idle")]
    public float idleRoamRange = 50.0f;
    [Tooltip("Seconds to wait after finishing an attack")]
    public float attackCooldown = 5.0f;
    [Tooltip("Ground and player layer masks")]
    public LayerMask groundMask, playerMask;
    [Tooltip("Radius of this object")]
    public float skinRadius = 0.5f;

    public GameObject projectile;

    public GameObject projectileSource;

    public float projectileSpeed = 10.0f;

    public int salvoNum;

    public AudioSource missileLauncherFireSFX;

    

    // PRIVATE VARIABLES
    // storage for this object's origin point at awake, its current navpoint and the player's previous known position
    Vector3 originPoint, navPoint, playerLastPos;
    // transform component of the player
    Transform player, mainCam;
    // playercombat script attached to player
    CharacterCombatController playerController;
    // this object's attached navmesh agent component
    
    // bools used for state machine decisionmaking
    public bool idlePointSet, attacking, playerInAttackRange, playerInDangerRange, canTeleport, attackOnCooldown, attackPointSet;
    // time delay between reaching an idle point and choosing a new one
    float timeToNewIdlePoint, damageCooldown, timeToTeleportRefresh;
    // time the next attack can be made, initialised at -1 to allow the first attack
    float attackCooldownTime = -1.0f;
    // currently unused: angle which allows the AI to vary its attack vector, in the event that the unit should 'lag' its attack the more the player moves
    float attackAngle;
    CapsuleCollider _coll;

    private void Awake()
    {
        originPoint = transform.position;
        player = GameObject.Find("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
        playerController = player.GetComponent<CharacterCombatController>();
        mainCam = Camera.main.gameObject.transform;
        _coll = GetComponent<CapsuleCollider>();
        dangerRange = attackRange / 2;
        List<Material> materialList = new List<Material>();
        if (meshRenderer.Length > 0)
        {
            foreach (MeshRenderer renderer in meshRenderer)
            {
                foreach(Material mat in renderer.materials)
                {
                    materialList.Add(mat);
                }
            }
            dissolveMaterials = materialList.ToArray();
        }
    }

    void Update()
    {
        if (GameController.instance.paused)
        {
            navAgent.enabled = false;
            return;
        }
        navAgent.enabled = true;

        if (hasSeenPlayer)
        {
            navAgent.updateRotation = false;
            Vector3 targetDir = player.position - transform.position;
            float singleStep = .9f * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        if (Time.time > attackCooldownTime) attackOnCooldown = false;
        if (Time.time > timeToTeleportRefresh) canTeleport = true;

        // check if player is in range of attack ability, or if the player has been spotted
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        playerInDangerRange = Physics.CheckSphere(transform.position, dangerRange, playerMask);
        if (!hasSeenPlayer) hasSeenPlayer = CheckSeenPlayer();

        // basic state machine for choosing AI action
        ActionDecision();
    }

    /// <summary>
    /// Finite state machine, self explanatory behaviours
    /// </summary>
    void ActionDecision()
    {
        if (!hasSeenPlayer && !attacking)
        {
            Idle();
            return;
        }
        if (!playerInAttackRange && !attacking)
        {
            NavigateToPlayer();
            return;
        }
        if (attackOnCooldown)
        {
            NavigateAwayFromPlayer();
            return;
        }
        if (playerInAttackRange && playerInDangerRange && attackOnCooldown)
        {
            NavigateAwayFromPlayer();
            return;
        }
        if (!attackOnCooldown && playerInAttackRange)
        {
            Attack();
            return;
        }
        NavigateAwayFromPlayer();
    }

    /// <summary>
    /// Performs related attack move
    /// </summary>
    protected override void Attack()
    {
        attackCooldownTime = Time.time + attackCooldown;
        attackOnCooldown = true;
        ShootProjectile();
    }

    void ShootProjectile()
    {
        StartCoroutine(FireSalvo());
    }

    public GameObject targetAreaMarker;

    /// <summary>
    /// Co-routine firing a salvo of missiles at the player's position, and marking the target area with a visual effect so the player can see where they need to avoid.
    /// </summary>
    /// <returns></returns>
    IEnumerator FireSalvo()
    {
        for(int i = 0; i < salvoNum; i++)
        {
            animator.ResetTrigger("Damaged");
            animator.SetTrigger("Fire");
            missileLauncherFireSFX.Play();
            Vector2 randomPoint2D = Random.insideUnitCircle * (3 * FirstPersonMovementController.instance.percentageMovementOfMax);
            Vector3 randomPoint = player.position + new Vector3(randomPoint2D.x, player.position.y, randomPoint2D.y);
            Ray ray = new Ray(randomPoint, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 2, LayerMask.GetMask("Ground")))
            {
                GameObject marker = Instantiate(targetAreaMarker, new Vector3(hit.point.x, hit.point.y + 1f, hit.point.z), transform.rotation);
                GameObject _projectile = Instantiate(projectile, projectileSource.transform.position, projectileSource.transform.rotation);
                _projectile.GetComponent<HomingMissile>().Initialise(hit.point, 15.0f, damage, attackRange, marker);
                
            } else
            {
                GameObject _projectile = Instantiate(projectile, projectileSource.transform.position, projectileSource.transform.rotation);
                _projectile.GetComponent<HomingMissile>().Initialise(player.position, 15.0f, damage, attackRange, null);
            }
            yield return new WaitForSeconds(.15f);
        }
    }

    /// <summary>
    /// The generic idle state, where the enemy patrols around their spawn-in point.
    /// </summary>
    protected override void Idle()
    {
        // roam around the spawnpoint
        // if there is currently a set delay to finding a new nav point, cancel movement
        if (Time.time < timeToNewIdlePoint) return;

        // find and assign new nav point, and/or navigate to the nav point
        if (!idlePointSet) SearchIdleWalkPoint();
        if (idlePointSet) navAgent.SetDestination(navPoint);

        Vector3 distanceToWalkPoint = transform.position - navPoint;

        if (distanceToWalkPoint.magnitude < 1)
        {
            idlePointSet = false;
            float randomTime = Random.Range(1.0f, 3.0f);
            timeToNewIdlePoint = Time.time + randomTime;
        }
    }

    /// <summary>
    /// Method to navigate to a set distance away from the player.
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
    /// Identifies a point closest to the AI which is on the circumference of a circle of radius attackRange * 2 to navigate toward and hold position while attack is on cooldown.
    /// </summary>
    void NavigateAwayFromPlayer()
    {
        if (Vector3.Distance(playerLastPos, player.position) < 0.5f) return;

        
        Vector3 travelDir = player.position - transform.position;
        Vector3 finalDir = (travelDir.normalized * attackRange);
        Vector3 targetPos = player.position - finalDir;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(targetPos, out hit, 4.0f, NavMesh.AllAreas))
        {
            navAgent.SetDestination(hit.position);
        }
    }

    /// <summary>
    /// Determines whether the player has been spotted.
    /// </summary>
    /// <returns></returns>
    private bool CheckSeenPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        if (distanceToPlayer > sightRange) return false;

        if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, groundMask))
        {
            //print("Seen player");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Finds a valid point within range of the spawn position to roam towards.
    /// </summary>
    private void SearchIdleWalkPoint()
    {
        float randomZ = Random.Range(-idleRoamRange, idleRoamRange);
        float randomX = Random.Range(-idleRoamRange, idleRoamRange);

        navPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(navPoint, -transform.up, 2f, groundMask))
        {
            idlePointSet = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (!displayDebugGizmos) return;

        Vector3 travelDir = player.position - transform.position;
        Vector3 finalDir = (travelDir.normalized * attackRange);
        Vector3 targetPos = player.position - finalDir;

        NavMeshHit hit;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPos, .5f);
        Gizmos.DrawLine(transform.position, targetPos);
    }
}
