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
    public GameObject attackMonsterPrefab; // Reference to the attack monster prefab

    [Header("AI Behavior")]
    public float detectionRange = 10f;
    public float attackRange = 3f;
    public float rotationSpeed = 5f;
    public GameObject monsterModel;
    private int currentWaypointIndex = 0;
    public float waypointThreshold = 1.0f;
    public bool isPatrolling = true;
    private bool isSearching = false;
    public float searchDuration = 2f;

    [Header("Footstep Settings")]
    public float footstepInterval = 0.5f;
    private float footstepTimer = 0f;

    [Header("AI Sounds")]
    public AudioClip walkingSound;
    public AudioClip runningSound;

    private StarterAssets.FirstPersonController playerController;
    private bool isAttacking = false;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (player != null) playerController = player.GetComponent<StarterAssets.FirstPersonController>();

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
            TriggerAttack();
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
        if (waypoints.Length == 0 || !isPatrolling || isSearching) return;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.speed = 3.2f;
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            agent.isStopped = false;
            agent.updatePosition = true;

            if (!agent.hasPath || agent.remainingDistance < waypointThreshold)
            {
                StartCoroutine(SearchRoutine());
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

        animator.SetBool("isSearching", true);
        animator.SetBool("isWalking", false);

        yield return new WaitForSeconds(searchDuration);

        agent.enabled = true;

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

    void TriggerAttack()
    {
        isAttacking = true;

        // Disable this monster
        monsterModel.SetActive(false);
        agent.isStopped = true;
        agent.enabled = false;

        // Lock player controls
        if (playerController != null)
        {
            playerController.LockControls(true);
        }

        // Find the attack monster (if it's in the scene)
        AttackShadowStalkerAI attackAI = FindAnyObjectByType<AttackShadowStalkerAI>(FindObjectsInactive.Include);

        if (attackAI != null)
        {
            // Position it correctly
            Camera playerCamera = Camera.main;
            attackAI.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 1.5f;

            // Activate it
            attackAI.StartAttack(playerCamera, playerController);
        }
        else if (attackMonsterPrefab != null) // Fallback to instantiation
        {
            // Instantiate if not found in scene
            GameObject attackMonster = Instantiate(
                attackMonsterPrefab,
                Camera.main.transform.position + Camera.main.transform.forward * 1.5f,
                Quaternion.identity);

            // Get the AttackShadowStalkerAI component and activate it
            attackAI = attackMonster.GetComponent<AttackShadowStalkerAI>();
            if (attackAI != null)
            {
                attackAI.StartAttack(Camera.main, playerController);
            }
        }
    }
    void ChaseTarget()
    {
        if (agent == null || !agent.enabled)
        {
            Debug.LogWarning("NavMeshAgent is null or disabled during chase");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent is not on NavMesh during chase, attempting to relocate");

            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError("Failed to find valid NavMesh position during chase");
                return;
            }
        }

        agent.speed = 7f;
        agent.isStopped = false;
        agent.SetDestination(player.position);
        agent.updatePosition = true;

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);

        PlayFootstepSound();
    }

    void UpdateAnimator()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        Vector3 movementDirection = agent.velocity;
        bool isMoving = movementDirection != Vector3.zero;

        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isRunning", isMoving && agent.hasPath && agent.velocity.magnitude > 3.5f);
    }

    void PlayFootstepSound()
    {
        if (footstepTimer > 0) return;

        AudioClip clipToPlay = agent.speed > 3.5f ? runningSound : walkingSound;
        SoundFXManager.Instance.PlaySoundFXClip(clipToPlay, transform, 1f);

        footstepTimer = footstepInterval;
    }
}