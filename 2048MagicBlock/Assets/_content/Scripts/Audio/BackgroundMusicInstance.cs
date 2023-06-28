using System.Collections;
using UnityEngine;

public class BackgroundMusicInstance : MonoBehaviour
{
    [Header("Tag")]
    [SerializeField] private string createdTag;

    public AudioClip[] musicTracks; // An array of audio clips for the music tracks
    private AudioSource audioSource;
    private float delayBetweenTracks = 10f; // The delay between tracks in seconds
    private float trackStartTime;
    private bool isPaused = false;
    private float fadeTime = 0f;

    [HideInInspector] public static BackgroundMusicInstance Instance; 

    private void Awake()
    {
        GameObject obj = GameObject.FindWithTag(this.createdTag);
        if (obj != null)
            Destroy(this.gameObject);
        else
        {
            this.gameObject.tag = this.createdTag;
            DontDestroyOnLoad(this.gameObject);
        }

        Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        delayBetweenTracks = Random.Range(
            CellLogic.Instance.balance.pauseBetweenMusicMin, 
            CellLogic.Instance.balance.pauseBetweenMusicMax
            );
        trackStartTime = Time.time;
        StartCoroutine(PlayTrackCheck());
    }

    private IEnumerator PlayTrackCheck()
    {
        while (true)
        {
            if (!audioSource.isPlaying 
                && Time.time - trackStartTime >= delayBetweenTracks
                && GameManager.Instance._canPlayMusicSetting)
            {
                PlayTrack();
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void PlayTrack()
    {
        if (GameManager.Instance._canPlayMusicSetting)
        {
            audioSource.clip = musicTracks[Random.Range(0, musicTracks.Length)];
            audioSource.Play();
            trackStartTime = Time.time;
            delayBetweenTracks = Random.Range(
                CellLogic.Instance.balance.pauseBetweenMusicMin,
                CellLogic.Instance.balance.pauseBetweenMusicMax
                );
        }
    }

    public void PauseMusic()
    {
        audioSource.volume = 0;
        isPaused = true;
    }

    public void ContinueMusic()
    {
        audioSource.volume = 1f;
        isPaused = false;
    }
}
