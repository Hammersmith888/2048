using UnityEngine;
using DG.Tweening;
using System.Collections;

public class LightLogic : MonoBehaviour
{
    [SerializeField] private CanvasGroup _first, _second, _third, _fourth;
    private Tween _tween;

    private void Awake()
    {
        _first.alpha = 0;
        _second.alpha = 0;
        _third.alpha = 0;
        _fourth.alpha = 0;
    }

    public void StartLight()
    {
        StartCoroutine(LightCoroutine());
    }

    private IEnumerator LightCoroutine()
    {
        LightFirst();
        yield return new WaitForSeconds(0.26f);
        LightSecond();
        yield return new WaitForSeconds(0.26f);
        LightThird();
        yield return new WaitForSeconds(0.26f);
        LightFourth();
    }


    private void LightFirst()
    {
        _tween = _first.DOFade(1, 0.5f).SetLoops(2, LoopType.Yoyo);
        _tween.onComplete += () =>
        {
            StartCoroutine(LightCoroutine());
        };
    }
    private void LightSecond()
    {
        _second.DOFade(1, 0.5f).SetLoops(2, LoopType.Yoyo);
    }
    private void LightThird()
    {
        _third.DOFade(1, 0.5f).SetLoops(2, LoopType.Yoyo);
    }
    private void LightFourth()
    {
        _fourth.DOFade(1, 0.5f).SetLoops(2, LoopType.Yoyo);
    }
    private void OnDisable()
    {
        _tween.Kill();
    }

    private void OnEnable()
    {
        StartLight();
    }
}
