using TMPro;
using UnityEngine;

public class TimerForSpecialOffer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timerText2;
    public float timeRemaining = 172800f; //172800f = 48 hours in seconds
    public float timeRemainingInitial = 172800f; //172800f = 48 hours in seconds

    private void Awake()
    {
        if (PlayerPrefs.HasKey("TimeRemaining"))
        {
            timeRemaining = PlayerPrefs.GetFloat("TimeRemaining");
        }

        if (PlayerPrefs.HasKey("StartTime"))
        {
            float startTime = PlayerPrefs.GetFloat("StartTime");
            timeRemaining -= startTime - Time.time;
        }
        else
        {
            PlayerPrefs.SetFloat("StartTime", Time.time);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("TimeRemaining", timeRemaining);
        PlayerPrefs.SetFloat("StartTime", Time.time);
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            int hours = (int)(timeRemaining / 3600f);
            int minutes = (int)((timeRemaining % 3600f) / 60f);
            int seconds = (int)(timeRemaining % 60f);

            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            timerText2.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
        else
        {
            timeRemaining = timeRemainingInitial;
        }
    }
}
