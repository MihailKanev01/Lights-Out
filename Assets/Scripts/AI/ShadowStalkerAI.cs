using UnityEngine.AI;
using UnityEngine;

public class ShadowStalkerAI : MonoBehaviour
{
    [Header("AI Settings")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;
    public Transform[] waypoints;

    [Header("AI Behavior")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float rotationSpeed = 5f;
    public GameObject monsterModel;
    private int currentWaypointIndex = 0;
    public float waypointThreshold = 1.0f;
    public bool isPatrolling = true;

    [Header("Flashlight Settings")]
    public Light playerFlashlight;
    public float flashlightAngle = 30f;
    public float flashlightDistance = 10f;

    [Header("Sounds")]
    public AudioClip[] fakeSounds;

    private bool isVanished = false;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (playerFlashlight == null) Debug.LogError("Flashlight reference is missing!");

        // Remove spawn logic
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position; // Start at first waypoint
        }
    }

    void Update()
    {
        if (player == null || isVanished) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackTarget();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChaseTarget();
        }
        else
        {
            Patrol();
        }

        UpdateAnimator();

        if (playerFlashlight.enabled && IsInFlashlight())
        {
            playerFlashlight.enabled = false;
            Vanish();
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0 || !isPatrolling) return;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.speed = 3.2f;
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            agent.isStopped = false;
            if (!agent.hasPath || agent.remainingDistance < waypointThreshold)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
    }

    bool IsInFlashlight()
    {
        if (!playerFlashlight.enabled) return false;

        Vector3 directionToMonster = transform.position - playerFlashlight.transform.position;
        float angle = Vector3.Angle(playerFlashlight.transform.forward, directionToMonster);
        float distance = directionToMonster.magnitude;

        Debug.Log($"Flashlight Check - Angle: {angle}, Distance: {distance}");

        return angle < flashlightAngle / 2f && distance < flashlightDistance;
    }

    void Vanish()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        isVanished = true;
        monsterModel.SetActive(false);

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("Attack", false);

        Debug.Log("Monster vanished!");
    }

    void AttackTarget()
    {
        agent.isStopped = true;
        animator.SetBool("Attack", true);

        Debug.Log("Monster is attacking!");

        Invoke("ResetAttack", 1.5f);
    }

    void ResetAttack()
    {
        animator.SetBool("Attack", false);

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    void ChaseTarget()
    {
        agent.speed = 7f;
        agent.isStopped = false;
        agent.SetDestination(player.position);

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
    }

    void UpdateAnimator()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        Vector3 movementDirection = agent.velocity;
        bool isMoving = movementDirection != Vector3.zero;

        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isRunning", isMoving && agent.hasPath && agent.velocity.magnitude > 3.5f);
        animator.SetBool("Attack", agent.isStopped);
    }
}
