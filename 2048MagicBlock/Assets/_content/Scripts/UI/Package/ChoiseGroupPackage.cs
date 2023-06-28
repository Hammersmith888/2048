using DG.Tweening;
using UnityEngine;

public class ChoiseGroupPackage : MonoBehaviour
{
    [SerializeField] private ChoiseGroupAnimation _block, _themes, _collections, _effects;

    [SerializeField] private GameObject _blockObject, _themesObject, _collectionsObject, _effectsObject;

    private CanvasGroup _blockCanvas, _themesCanvas, _collectionsCanvas, _effectsCanvas;

    private bool _state;

    private void Awake()
    {
        _blockCanvas = _blockObject.GetComponent<CanvasGroup>();
        _themesCanvas = _themesObject.GetComponent<CanvasGroup>();
        _collectionsCanvas = _collectionsObject.GetComponent<CanvasGroup>();
        _effectsCanvas = _effectsObject.GetComponent<CanvasGroup>();
    }

    public void StartOpenPackage()
    {
        _block.Active();
        AnalyticsManager.Instance.LogEvent("package_tap");
        _themes.NotActive();
        _collections.NotActive();
        _effects.NotActive();

        _blockObject.SetActive(true);
        _blockCanvas.alpha = 1;

        _themesCanvas.alpha = 0;
        _collectionsCanvas.alpha = 0;
        _effectsCanvas.alpha = 0;

        _themesObject.SetActive(false);
        _collectionsObject.SetActive(false);
        _effectsObject.SetActive(false);
    }

    public void OpenBlock()
    {
        if (_state == false)
        {
            _state = true;

            _block.Active();

            _themes.NotActive();
            _collections.NotActive();
            _effects.NotActive();

            _themesCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _themesObject.SetActive(false);
            };

            _collectionsCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _collectionsObject.SetActive(false);
            };

            _effectsCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _effectsObject.SetActive(false);
                _state = false;

                _blockObject.SetActive(true);

                _blockCanvas.DOFade(1, 0.25f);
            };
        }
    }

    public void OpenThemes()
    {
        if (_state == false)
        {
            _state = true;

            _themes.Active();

            _block.NotActive();
            _collections.NotActive();
            _effects.NotActive();

            _blockCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _blockObject.SetActive(false);
            };

            _collectionsCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _collectionsObject.SetActive(false);
            };

            _effectsCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _effectsObject.SetActive(false);
                _state = false;

                _themesObject.SetActive(true);

                _themesCanvas.DOFade(1, 0.25f);
            };
        }

    }

    public void OpenCollections()
    {
        if (_state == false)
        {
            _state = true;

            _collections.Active();

            _block.NotActive();
            _themes.NotActive();
            _effects.NotActive();

            _blockCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _blockObject.SetActive(false);
            };

            _themesCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _themesObject.SetActive(false);
            };

            _effectsCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _effectsObject.SetActive(false);
                _state = false;

                _collectionsObject.SetActive(true);

                _collectionsCanvas.DOFade(1, 0.25f);
            };
        }
    }

    public void OpenEffects()
    {
        if (_state == false)
        {
            _state = true;

            _effects.Active();

            _block.NotActive();
            _themes.NotActive();
            _collections.NotActive();

            _blockCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _blockObject.SetActive(false);
            };

            _themesCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _themesObject.SetActive(false);
            };

            _collectionsCanvas.DOFade(0, 0.25f).onComplete += () =>
            {
                _collectionsObject.SetActive(false);
                _state = false;

                _effectsObject.SetActive(true);

                _effectsCanvas.DOFade(1, 0.25f);
            };
        }
    }
}
