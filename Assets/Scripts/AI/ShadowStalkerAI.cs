using UnityEngine.AI;
using UnityEngine;

public class ShadowStalkerAI : MonoBehaviour
{
    [Header("AI Settings")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform[] spawnPoints;
    public Transform player;

    [Header("AI Behavior")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float rotationSpeed = 5f;
    public GameObject monsterModel;

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

        SpawnAtRandomLocation();
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
            Idle();
        }

        UpdateAnimator();

        if (playerFlashlight.enabled && IsInFlashlight())
        {
            playerFlashlight.enabled = false;
            Vanish();
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

        Invoke(nameof(RespawnMonster), 2f);
    }

    void RespawnMonster()
    {
        isVanished = false;
        SpawnAtRandomLocation();

        if (agent != null)
        {
            agent.enabled = true;
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
        }

        monsterModel.SetActive(true);

        Debug.Log("Monster respawned and is now idle.");

        if (fakeSounds.Length > 0)
            SoundFXManager.Instance.PlayRandomSoundFXClip(fakeSounds, transform, 1f);
    }
    void SpawnAtRandomLocation()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[randomIndex].position;

        spawnPosition.y = 1.0f; 

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, 2.0f, NavMesh.AllAreas))
        {
            agent.enabled = false;
            transform.position = hit.position;
            agent.enabled = true;

            agent.isStopped = true;
            agent.ResetPath();

            Debug.Log($"Monster spawned at a valid NavMesh location: {hit.position}");
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
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void Idle()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        Debug.Log("Monster is idle.");
    }

    void HandleRotation()
    {
        if (agent.velocity.sqrMagnitude > 0.01f) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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

}