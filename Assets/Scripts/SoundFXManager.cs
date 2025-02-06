using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform transform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, transform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform transform, float volume)
    {
        int rand = Random.Range(0, audioClip.Length);
        AudioSource audioSource = Instantiate(soundFXObject, transform.position, Quaternion.identity);
        audioSource.clip = audioClip[rand];
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip[rand].length);
    }
}
