using UnityEngine;

public class SoundMimic : MonoBehaviour
{
    public AudioClip[] fakeSounds;
    private float soundTimer = 0f;
    private float soundInterval = 15f;

    private void Update()
    {
        soundTimer += Time.deltaTime;

        if (soundTimer > soundInterval)
        {
            PlayFakeSound();
            soundTimer = 0f;
        }
    }

    private void PlayFakeSound()
    {
        if (fakeSounds.Length > 0)
            SoundFXManager.Instance.PlayRandomSoundFXClip(fakeSounds, transform, 1f);
    }
}
