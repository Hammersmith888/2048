using UnityEngine;

public class AudioPlayOnCollison : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlaySound();
    }
}
