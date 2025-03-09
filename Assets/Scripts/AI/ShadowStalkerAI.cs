using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine.SceneManagement;

public class ShadowStalkerAI : MonoBehaviour
{
    [Header("AI Settings")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;
    public Transform[] waypoints;
    public Camera playerCamera;

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

    [Header("Jumpscare Settings")]
    public float cameraShakeIntensity = 10f;
    public float cameraShakeDuration = 0.5f;
    public float cameraRotationSpeed = 5f;
    public float jumpscareDelay = 1.5f;

    [Header("Footstep Settings")]
    public float footstepInterval = 0.5f;
    private float footstepTimer = 0f;

    [Header("AI Sounds")]
    public AudioClip walkingSound;
    public AudioClip runningSound;
    public AudioClip attackSound;

    [Header("AI Attack Settings")]
    public float attackCooldown = 2f;
    private bool isAttacking = false;

    private StarterAssets.FirstPersonController playerController;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (playerCamera == null) playerCamera = Camera.main;
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
                StartCoroutine(SearchRoutine()); // Start the search delay
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

    void AttackTarget()
    {
        if (isAttacking) return;

        isAttacking = true;
        isSearching = false;

        // Only stop the agent if it's valid
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        animator.SetBool("isAttacking", true);
        animator.SetBool("isSearching", false);
        PlayAttackSound();

        // Lock player controls if available
        if (playerController != null)
        {
            playerController.LockControls(true);
        }

        // Start the jumpscare sequence
        StartCoroutine(GameOverSequence());
    }
    IEnumerator GameOverSequence()
    {
        if (playerCamera != null)
        {
            // First shake the camera slightly
            for (int i = 0; i < 5; i++)
            {
                playerCamera.transform.Rotate(Random.Range(-cameraShakeIntensity, cameraShakeIntensity),
                                             Random.Range(-cameraShakeIntensity, cameraShakeIntensity), 0);
                yield return new WaitForSeconds(0.05f);
            }

            // Then smoothly rotate camera to face the monster
            float duration = cameraShakeDuration;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                Vector3 direction = (transform.position - playerCamera.transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, lookRotation, Time.deltaTime * cameraRotationSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure it's directly facing monster at the end
            Vector3 finalDirection = (transform.position - playerCamera.transform.position).normalized;
            playerCamera.transform.rotation = Quaternion.LookRotation(finalDirection);
        }

        // Pause for the jumpscare effect
        yield return new WaitForSeconds(jumpscareDelay);

        // Finally trigger game over
        GameOver();
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        SceneManager.LoadScene("GameOverScene");
    }

    void ChaseTarget()
    {
        // First check if agent is valid and on NavMesh before using it
        if (agent == null || !agent.enabled)
        {
            Debug.LogWarning("NavMeshAgent is null or disabled during chase");
            return;
        }

        // Check if agent is on NavMesh
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent is not on NavMesh during chase, attempting to relocate");

            // Try to find a valid position on NavMesh and warp to it
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

        // Now that we've verified the agent is valid, proceed with chase
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
        animator.SetBool("Attack", agent.isStopped);
    }

    void PlayFootstepSound()
    {
        if (footstepTimer > 0) return;

        AudioClip clipToPlay = agent.speed > 3.5f ? runningSound : walkingSound;
        SoundFXManager.Instance.PlaySoundFXClip(clipToPlay, transform, 1f);

        footstepTimer = footstepInterval;
    }

    void PlayAttackSound()
    {
        SoundFXManager.Instance.PlaySoundFXClip(attackSound, transform, 1f);
    }
}