using System.Collections;

using UnityEngine;

public class RandomPlaySparkle : MonoBehaviour
{
    private ParticleSystem _sparkle;
    [SerializeField] private bool _state;

    private Coroutine sparklePlaying;

    private void Awake()
    {
        _sparkle = gameObject.GetComponent<ParticleSystem>();

        if (_state == true) StartCoroutine(CoroutinePlaySparkle());
    }

    private IEnumerator CoroutinePlaySparkle()
    {
        float delay = Random.Range(0.1f, 15f);
        yield return new WaitForSeconds(delay);
        _sparkle.Play();
        sparklePlaying = StartCoroutine(CoroutinePlaySparkle());
    }

    public void StartPlaySparkle()
    {
        sparklePlaying = StartCoroutine(CoroutinePlaySparkle());
    }    

    public void StopPlaySparkle()
    {
        if(sparklePlaying != null)
            StopCoroutine(sparklePlaying);

        if (_sparkle == null) return; //Выдавало ошибку https://imgur.com/a/qKHOqbd
        _sparkle.Stop();
        _sparkle.Clear();
    }    
}
