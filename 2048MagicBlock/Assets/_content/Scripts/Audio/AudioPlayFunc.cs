using UnityEngine;

public class AudioPlayFunc : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        audioSource.enabled = false;

        Invoke("EnableAudioSource", 1f);
    }

    public void PlaySound()
    {
        if (GameManager.Instance._canPlaySoundSetting)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
    public void PlaySound(float invokeTime)
    {
        Invoke("PlaySound", invokeTime);
    }

    private void EnableAudioSource()
    {
        audioSource.enabled = true;
    }
}
