using UnityEngine;
using UnityEngine.AI;

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
            AttackTarget();
        else if (distanceToPlayer <= detectionRange)
            ChaseTarget();
        else
            Idle();

        UpdateAnimator();
        HandleRotation();

        // Check if the monster is in the flashlight's beam
        if (playerFlashlight.enabled && IsInFlashlight())
        {
            Vanish();
        }
    }

    bool IsInFlashlight()
    {
        Vector3 directionToLight = transform.position - playerFlashlight.transform.position;
        float angle = Vector3.Angle(playerFlashlight.transform.forward, directionToLight);
        return angle < flashlightAngle && Vector3.Distance(playerFlashlight.transform.position, transform.position) < flashlightDistance;
    }

    void Vanish()
    {
        isVanished = true;
        monsterModel.SetActive(false);
        agent.isStopped = true;
        Debug.Log("Monster vanished!");

        Invoke(nameof(RespawnMonster), 2f);
    }

    void RespawnMonster()
    {
        isVanished = false;
        SpawnAtRandomLocation();
        monsterModel.SetActive(true);
        agent.isStopped = false;
        Debug.Log("Monster respawned!");

        if (fakeSounds.Length > 0)
            SoundFXManager.Instance.PlayRandomSoundFXClip(fakeSounds, transform, 1f);
    }

    void SpawnAtRandomLocation()
    {
        if (spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            transform.position = spawnPoints[randomIndex].position;
        }
    }

    void AttackTarget()
    {
        agent.isStopped = true;
    }

    void ChaseTarget()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void Idle()
    {
        agent.isStopped = true;
    }

    void HandleRotation()
    {
        if (agent.velocity.sqrMagnitude > 0.01f) // If moving
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateAnimator()
    {
        Vector3 movementDirection = agent.velocity;
        bool isMoving = movementDirection != Vector3.zero;

        // Walking
        animator.SetBool("isWalking", isMoving);

        // Running (if the agent has a path and is moving fast)
        animator.SetBool("isRunning", isMoving && agent.hasPath && agent.velocity.magnitude > 3.5f);

        // Attacking (if the agent is stopped and in attack range)
        animator.SetBool("Attack", agent.isStopped);

        // Rotate towards movement direction
        if (isMoving)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, agent.angularSpeed * Time.deltaTime);
        }
    }
}
