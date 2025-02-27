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
    public float searchDuration = 4f;

    [Header("Footstep Settings")]
    public float footstepInterval = 0.5f; // Time between footstep sounds
    private float footstepTimer = 0f;


    [Header("AI Sounds")]
    public AudioClip walkingSound;
    public AudioClip runningSound;
    public AudioClip attackSound;

    [Header("AI Attack Settings")]
    public float attackCooldown = 2f; // Cooldown time in seconds
    private bool isAttacking = false; // Tracks if attack is in progress

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
        footstepTimer -= Time.deltaTime;

        Flashlight playerflashlight = player.GetComponent<Flashlight>();
        if (playerflashlight != null && playerflashlight.GetSanity() <= 0)
        {
            Debug.Log("Player sanity is 0! Monster is chasing!");
            StopAllCoroutines();
            agent.enabled = true;
            agent.isStopped = false;
            ChaseTarget();
            return;
        }
        if (!isSearching && agent.remainingDistance > 0f && agent.remainingDistance <= waypointThreshold && !agent.pathPending)
        {
            StartCoroutine(SearchRoutine());
            return;
        }

        if (distanceToPlayer <= attackRange && !isAttacking)
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
            agent.updatePosition = true;

            if (!agent.hasPath || agent.remainingDistance < waypointThreshold)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
            PlayFootstepSound();
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
        yield return null;

        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.Warp(hit.position);
            }
        }
                agent.updatePosition = true;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
                agent.isStopped = false;

            isSearching = false;
    }

    void AttackTarget()
    {
        if (isAttacking) return; // Prevent multiple attacks at once

        isAttacking = true;
        agent.isStopped = true;

        animator.SetBool("Attack", true);
        PlayAttackSound();
        Debug.Log("Monster is attacking!");

        Invoke("ResetAttack", 1.5f); // Wait for animation to finish
        Invoke("EnableAttack", attackCooldown); // Re-enable attack after cooldown
    }
    void EnableAttack()
    {
        isAttacking = false; // Allow next attack
    }
    void ResetAttack()
    {
        animator.SetBool("Attack", false);
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    void ChaseTarget()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.speed = 7f;
            agent.isStopped = false;
            agent.SetDestination(player.position);
            agent.enabled = true;
            agent.updatePosition = true;
            agent.Warp(player.position);

            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);

            PlayFootstepSound();
        }
        else
        {
            Debug.LogError("ChaseTarget() called but agent is not on NavMesh!");
        }
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

    void PlayFootstepSound()
    {
        if (footstepTimer > 0) return; // Prevent spamming footstep sounds

        AudioClip clipToPlay = agent.speed > 3.5f ? runningSound : walkingSound;
        SoundFXManager.Instance.PlaySoundFXClip(clipToPlay, transform, 1f);

        footstepTimer = footstepInterval; // Reset timer
    }

    void PlayAttackSound()
    {
        SoundFXManager.Instance.PlaySoundFXClip(attackSound, transform, 1f);
    }
}
