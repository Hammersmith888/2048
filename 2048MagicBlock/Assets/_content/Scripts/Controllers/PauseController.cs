using System;
using System.Collections;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private float _delay;
    private void Start()
    {
       // PausePanel.PauseDelay += StartDelayPause;
    }
    private void OnEnable()
    {
        PausePanel.PauseDelay += StartDelayPause;
    }
    private void OnDisable()
    {
        PausePanel.PauseDelay -= StartDelayPause;
    }

    private void StartDelayPause()
    {
        StartCoroutine(DelayTimerStart());
    }

    private IEnumerator DelayTimerStart()
    {
        yield return new WaitForSeconds(_delay);

        Debug.Log("Pause off");

        GameManager.Instance.isPause = false;
    }
}
