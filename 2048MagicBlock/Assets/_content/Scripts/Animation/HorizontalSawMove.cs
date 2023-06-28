using UnityEngine;
using DG.Tweening;
using System.Collections;

public class HorizontalSawMove : MonoBehaviour
{
    [SerializeField] private float _duration;

    [SerializeField] private float _end;

    [SerializeField] private ParticleSystem _sawdust;

    private CanvasGroup _image;

    private Coroutine _coorutine;
    private Tween _currentTween;

    private void Awake()
    {
        _image = gameObject.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        _coorutine = StartCoroutine(StartSaveMove());
    }

    private void OnDisable()
    {
        if (_coorutine != null)
        {
            StopCoroutine(_coorutine);
            _coorutine = null;
        }

        if(_currentTween != null)
        {
            _currentTween.Kill();
            _currentTween = null;
        }
    }

    private void SawMove()
    {
        _currentTween = transform.DOLocalMoveX(_end, _duration).SetLoops(6, LoopType.Yoyo);

        _currentTween.onComplete += () =>
        {
            _image.DOFade(0, 0.25f).SetDelay(0.1f);
        };
    }
        

    private IEnumerator StartSaveMove()
    {
        _image.DOFade(1, 0.25f);
        yield return new WaitForSeconds(0.3f);
        _sawdust.Play();
        SawMove();
        yield return new WaitForSeconds(1.25f);
    }
}
