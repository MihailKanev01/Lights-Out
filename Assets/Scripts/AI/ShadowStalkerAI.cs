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

    [Header("Sounds")]
    public AudioClip[] fakeSounds;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position; // Start at first waypoint
        }
    }

    void Update()
    {
        if (player == null) return;

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
