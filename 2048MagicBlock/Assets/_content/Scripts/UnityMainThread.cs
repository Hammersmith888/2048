using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class UnityMainThread : MonoBehaviour
{
    internal static UnityMainThread wkr;
    Queue<Action> jobs = new Queue<Action>();

    void Awake() {
        DontDestroyOnLoad(gameObject);
        wkr = this;
    }

    private void OnDestroy()
    {
        StopCoroutine(UpdateThread());
    }

    private void WaitNextFrame()
    {
        StartCoroutine(UpdateThread());
    }

    private IEnumerator UpdateThread() {

        yield return new WaitForEndOfFrame();

        if (jobs.Count > 0) 
            jobs.Dequeue().Invoke();
    }

    internal void AddJob(Action newJob) {
        jobs.Enqueue(newJob);

        WaitNextFrame();
    }
}