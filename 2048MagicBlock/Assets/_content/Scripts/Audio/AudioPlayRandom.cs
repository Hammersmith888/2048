using System.Collections;
using UnityEngine;

public class AudioPlayRandom : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioSource audioSource;

    private float _delayTime = 2f;
    private float _timeAtLastSound = 0;
    private int _lastIndexSound = 0;
    private int _currentIndexSound = 0;
    private bool _hasPlayRandomSound = true;

    public void PlaySound()
    {
        if (GameManager.Instance._canPlaySoundSetting)
        {
            _currentIndexSound = Random.Range(0, audioClips.Length);

            if (_hasPlayRandomSound)
            {
                audioSource.clip = audioClips[_currentIndexSound];
                _lastIndexSound = _currentIndexSound;
                StartCoroutine(TimerLastSound());
            }
            else
                audioSource.clip = audioClips[_lastIndexSound];

            audioSource.Play();
        }
    }
    public void PlaySound(float invokeTime)
    {
        Invoke("PlaySound", invokeTime);
    }
    private IEnumerator TimerLastSound()
    {
        bool whileActive = true;
        _hasPlayRandomSound = false;

        while (whileActive)
        {
            _timeAtLastSound += 0.1f;

            yield return new WaitForSeconds(0.1f);

            if (_timeAtLastSound > _delayTime)
            {
                _timeAtLastSound = 0;
                _hasPlayRandomSound = true;
                whileActive = false;
            }
        }
    }
}
