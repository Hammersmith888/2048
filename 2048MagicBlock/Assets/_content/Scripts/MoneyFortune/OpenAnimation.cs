using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;

public class OpenAnimation : MonoBehaviour
{

    [SerializeField] private GameObject _screen1, _screen2;
    [SerializeField] private CanvasGroup _shinning;

    private CanvasGroup _screen1Canvas, _screen2Canvas, _canavs;
    [SerializeField] private TextMeshProUGUI _timer;
    private Coroutine _timerWaiter;

    void Awake()
    {
        _screen1Canvas = _screen1.GetComponent<CanvasGroup>();
        _screen2Canvas = _screen2.GetComponent<CanvasGroup>();
        _canavs = gameObject.GetComponent<CanvasGroup>();

        _canavs.alpha = 0;
        _screen1Canvas.alpha = 0;
        _shinning.alpha = 0;
    }

    private void StartTimerWait()
    {
        _timerWaiter = StartCoroutine(TimerWaiter());
    }
    IEnumerator TimerWaiter()
    {
        _timer.gameObject.SetActive(true);
        for (int i=30;i>=0;i--)
        {
            //_timer.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        //_timer.gameObject.SetActive(false);
        CloseWindiw();
    }
    public void OpenFortyne(int index)
    {
       
        _screen1.SetActive(true);
        _screen1.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        _screen1.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetDelay(0.1f);

        _canavs.DOFade(1, 0.25f).onComplete += () =>
        {
            _screen1Canvas.DOFade(1, 0.25f);
            _shinning.DOFade(1, 0.25f).SetDelay(0.1f);
        };

        GameManager.Instance.isPause = true;

        _screen2.SetActive(false);

        GetComponent<BlockShow>().ShowImage(index);
    }
    
    public void OpenRoulette()
    {
        StartTimerWait();
        _screen1.transform.DOScale(new Vector3(0, 0, 0), 0.25f);

    
        _screen1Canvas.DOFade(0, 0.25f).onComplete += () =>
        {
            _screen1.SetActive(false);

            _screen2.SetActive(true);
            _screen2.transform.localScale = new Vector3(0f, 0f, 0f);
            _screen2.transform.DOScale(new Vector3(1, 1, 1), 0.25f).SetDelay(0.25f);

            _screen2Canvas.alpha = 0;
            _screen2Canvas.DOFade(1, 1f);
        };
    }

    public void CloseWindiw()
    {
        _canavs.DOFade(0, 0.25f).onComplete += () =>
        {
            gameObject.SetActive(false);
            GameManager.Instance.isPause = false;
        };

        _screen1Canvas.alpha = 0;
        _shinning.alpha = 0;
        CellLogic.state = false;
        //_timer.gameObject.SetActive(false);
        StopCoroutine(_timerWaiter);
    }
}
