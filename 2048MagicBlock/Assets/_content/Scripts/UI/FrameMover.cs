using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FrameMover : MonoBehaviour
{
    [SerializeField] private GameObject _frame;

    [SerializeField] private List<GameObject> _cube;

    private CanvasGroup _canvasFrame;

    private bool _state;

    void Awake()
    {
        StartGame();
    }

    public void StartGame()
    {

        _canvasFrame = _frame.GetComponent<CanvasGroup>();

        _canvasFrame.alpha = 1.0f;

        StartFrame();

        GameManager.Instance.isPause = true;
    }
    private void StartFrame()
    {
        _frame.transform.SetParent(_cube[0].transform);
        _frame.transform.position = _cube[0].transform.position;
    }

    public void Mover(int index)
    {
        GameManager.Instance._btnClickSound.PlaySound();

        if (_state == false)
        {
            if (index == 0) AnalyticsManager.Instance.LogEvent("start_from5");
            if (index == 1) AnalyticsManager.Instance.LogEvent("start_from10");
            if (index == 2) AnalyticsManager.Instance.LogEvent("start_from18");
            _state = true;
            _canvasFrame.DOFade(0, 0.1f).onComplete += () =>
            {
                _frame.transform.SetParent(_cube[index].transform);
                _frame.transform.position = _cube[index].transform.position;
                _canvasFrame.DOFade(0, 0.1f).onComplete += () =>
                {
                    _canvasFrame.DOFade(1, 0.1f).onComplete += () =>
                    {
                        _state = false;
                    };
                };
            };
        }
    }
}
