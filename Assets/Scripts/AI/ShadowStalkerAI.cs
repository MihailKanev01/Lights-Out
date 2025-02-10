using UnityEngine;
using UnityEngine.AI;

public class ShadowStalkerAI : MonoBehaviour
{
    public Transform player;
    public Light playerFlashlight;
    public Transform[] spawnPoints;
    public AudioClip[] fakeSounds;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (playerFlashlight.enabled && IsInLight())
        {
            playerFlashlight.enabled = false;
            DisappearAndRespawn();
        }
        else
            ChasePlayer();
    }

    private bool IsInLight()
    {
        Vector3 directionToLight = transform.position - playerFlashlight.transform.position;
        float angle = Vector3.Angle(playerFlashlight.transform.forward, directionToLight);
        return angle < 30f && Vector3.Distance(playerFlashlight.transform.position, transform.position) < 10f;
    }

    private void DisappearAndRespawn()
    {
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), 2f);
    }

    private void Respawn()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[randomIndex].position;
        gameObject.SetActive(true);

        if (fakeSounds.Length > 0)
            SoundFXManager.Instance.PlayRandomSoundFXClip(fakeSounds, transform, 1f);
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
}
