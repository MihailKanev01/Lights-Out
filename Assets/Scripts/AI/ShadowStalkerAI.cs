using UnityEngine.AI;
using UnityEngine;
using System.Collections;

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
    private bool isSearching = false;
    public float searchDuration = 2f;

    [Header("Sounds")]
    public AudioClip[] fakeSounds;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position; 
            currentWaypointIndex = 0;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            agent.isStopped = false;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (!isSearching && agent.remainingDistance > 0f && agent.remainingDistance <= waypointThreshold && !agent.pathPending)
        {
            StartCoroutine(SearchRoutine());
            return;
        }

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

    IEnumerator SearchRoutine()
    {
        isSearching = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updatePosition = false;
        agent.enabled = false;
        animator.SetTrigger("Search");
        animator.SetBool("isWalking", false);

        yield return new WaitForSeconds(searchDuration);

        agent.enabled = true;
        agent.updatePosition = true;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        agent.isStopped = false;
        isSearching = false;
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
