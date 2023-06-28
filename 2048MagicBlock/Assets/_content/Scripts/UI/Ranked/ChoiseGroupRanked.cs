using UnityEngine;
using DG.Tweening;

public class ChoiseGroupRanked : MonoBehaviour
{
    [SerializeField] private Transform _localTab, _globalTab, _localText, _globalText;

    [SerializeField] private CanvasGroup _localActive, _localNotActive, _globalActive, _globalNotActive;

    [SerializeField] private CanvasGroup _normal, _endless;

    [SerializeField] private GameObject _localNormal, _globalNormal, _localEndless, _globalEndless;

    [SerializeField] private CanvasGroup _localNormalPanel, _globalNormalPanel, _localEndlessPanel, _globalEndlessPanel;

    private bool _state;
    private int _index;

    private void Awake()
    {
        _localNormal.SetActive(true);
        _globalNormal.SetActive(false);
        _localEndless.SetActive(false);
        _globalEndless.SetActive(false);
        _index = 1;
    }

    public void OpenLocal()
    {
        _localTab.DOLocalMoveY(366, 0.5f);
        _localActive.DOFade(1, 0.5f);
        _localNotActive.DOFade(0, 0.5f);

        _globalTab.DOLocalMoveY(346, 0.5f);
        _globalActive.DOFade(0, 0.5f);
        _globalNotActive.DOFade(1, 0.5f);

        _localText.DOLocalMoveY(4.5f, 0.5f);
        _globalText.DOLocalMoveY(15.5f, 0.5f);

        if (_state == false)
        {
            _index = 1;
            FadePanel(_localNormalPanel, _localEndlessPanel, _globalEndlessPanel, _globalNormalPanel);
        }
            
        else
        {
            _index = 2;
            FadePanel(_localEndlessPanel, _globalEndlessPanel, _globalNormalPanel, _localNormalPanel);
        }
            
    }

    public void OpenGlobal()
    {
        _globalTab.DOLocalMoveY(366, 0.5f);
        _globalActive.DOFade(1, 0.5f);
        _globalNotActive.DOFade(0, 0.5f);

        _localTab.DOLocalMoveY(346, 0.5f);
        _localActive.DOFade(0, 0.5f);
        _localNotActive.DOFade(1, 0.5f);

        _localText.DOLocalMoveY(15.5f, 0.5f);
        _globalText.DOLocalMoveY(4.5f, 0.5f);

        if (_state == false)
        {
            _index = 3;
            FadePanel(_globalNormalPanel, _localEndlessPanel, _globalEndlessPanel, _localNormalPanel);
        }
            
        else
        {
            _index = 4;
            FadePanel(_globalEndlessPanel, _localEndlessPanel, _globalNormalPanel, _localNormalPanel);
        }
            
    }

    public void OpenNormal()
    {
        _normal.DOFade(1, 0.5f);
        _endless.DOFade(0, 0.5f);

        _state = false;
        if (_index == 2)
        {
            _index = 1;
            FadePanel(_localNormalPanel, _localEndlessPanel, _globalEndlessPanel, _globalNormalPanel);
        }
        else if(_index == 4)
        {
            _index = 3;
            FadePanel(_globalNormalPanel, _localEndlessPanel, _globalEndlessPanel, _localNormalPanel);
        }
    }

    public void OpenEndless()
    {
        _normal.DOFade(0, 0.5f);
        _endless.DOFade(1, 0.5f);

        _state = true;

        if (_index == 1)
        {
            _index = 2;
            FadePanel(_localEndlessPanel, _globalEndlessPanel, _globalNormalPanel, _localNormalPanel);
        }
        else if (_index == 3)
        {
            _index = 4;
            FadePanel(_globalEndlessPanel, _localEndlessPanel, _globalNormalPanel, _localNormalPanel);
        }
    }
    
    private void FadePanel(CanvasGroup showPanel, CanvasGroup hidePanelFirst, CanvasGroup hidePanelSecond, CanvasGroup hidePanelThird)
    {
        hidePanelFirst.DOFade(0, 0.5f).onComplete += () =>
        {
            OpenLogic(_index);
            showPanel.DOFade(1, 0.5f);
        };
        hidePanelSecond.DOFade(0, 0.5f);
        hidePanelThird.DOFade(0, 0.5f);
    }

    private void OpenLogic(int index)
    {
        switch (index)
        {
            case 1:
                SetActiveLogic(_localNormal, _globalEndless, _globalNormal, _localEndless);
                Debug.Log("Local - Normal");
                break;
            case 2:
                SetActiveLogic(_localEndless, _localNormal, _globalEndless, _globalNormal);
                Debug.Log("Local - Endless");
                break;
            case 3:
                SetActiveLogic(_globalNormal, _localNormal, _globalEndless, _localEndless);
                Debug.Log("Global - Normal");
                break;
            case 4:
                SetActiveLogic(_globalEndless, _globalNormal, _localNormal, _localEndless);
                Debug.Log("Global - Endless");
                break;
        }
    }

    private void SetActiveLogic(GameObject showPanel, GameObject hidePanelFirst, GameObject hidePanelSecond, GameObject hidePanelThird)
    {

        showPanel.SetActive(true);
        hidePanelFirst.SetActive(false);
        hidePanelSecond.SetActive(false);
        hidePanelThird.SetActive(false);
    }
}
