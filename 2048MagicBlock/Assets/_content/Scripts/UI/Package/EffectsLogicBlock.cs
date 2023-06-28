using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EffectsLogicBlock : MonoBehaviour
{
    [SerializeField] private int _index;
    [SerializeField] private GameObject _close;

    [SerializeField] private Text _costText;

    [SerializeField] private AnimationExpenseCoins _AnimationExpenseCoins;
    [SerializeField] private float _timeAnimationPlay = 0.4f;
    [SerializeField] private RectTransform _finalPositionCoinsAnim;
    [SerializeField] private float _editAnimPosFinalY = 150;

    private Button _button;

    private CanvasGroup _closeCanvas;

    private int _intState;
    private int _cost;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _closeCanvas = _close.GetComponent<CanvasGroup>();

        LoadState(_index);
        LoadCost(_index);
    }

    private void LoadState(int index)
    {
        if (index != 1)
        {
            if (!PlayerPrefs.HasKey("EffectState" + index))
            {
                PlayerPrefs.SetInt("EffectState" + index, 0);
                _intState = 0;
            }
            else
                _intState = PlayerPrefs.GetInt("EffectState" + index);

            StateDefinition(_intState);
        }

        else if (index == 1)
        {
            OpenBlock();
        }
    }

    private void StateDefinition(int stateInt)
    {
        if (_intState == 0)
            CloseBlock();
        else
            OpenBlock();
    }

    private void OpenBlock()
    {
        _closeCanvas.DOFade(0, 0.5f).onComplete += () =>
        {
            _button.interactable = true;
        };
    }

    private void CloseBlock()
    {
        _closeCanvas.alpha = 1;
        _close.SetActive(true);
        _button.interactable = false;
    }

    private void LoadCost(int index)
    {
        switch (index)
        {
            case 1:
                break;
            case 2:
                _cost = 30;
                _costText.text = _cost.ToString();
                break;
            case 3:
                _cost = 60;
                _costText.text = _cost.ToString();
                break;
            case 4:
                _cost = 120;
                _costText.text = _cost.ToString();
                break;
            case 5:
                _cost = 240;
                _costText.text = _cost.ToString();
                break;
            case 6:
                _cost = 480;
                _costText.text = _cost.ToString();
                break;
            case 7:
                _cost = 960;
                _costText.text = _cost.ToString();
                break;
            case 8:
                _cost = 1920;
                _costText.text = _cost.ToString();
                break;
            case 9:
                _cost = 3840;
                _costText.text = _cost.ToString();
                break;
        }
    }

    public void BuySkin()
    {
        if (_cost <= CoinPurse.Instance.CoinsCount)
        {
            AnalyticsManager.Instance.LogEvent("effect_buy");
            _AnimationExpenseCoins.SetFinalPosition(_finalPositionCoinsAnim);
            _AnimationExpenseCoins.SetEditAnimPosFinalY(_editAnimPosFinalY);
            _AnimationExpenseCoins.SetTimeToMove(_timeAnimationPlay);
            _AnimationExpenseCoins.StartAnimation(0);

            Invoke("SectionCodeBuySkin", _AnimationExpenseCoins.GetTimeToMove() + 1);
        }
    }

    public void SectionCodeBuySkin()
    {
        CoinPurse.Instance.CostSubtraction(_cost);

        OpenBlock();

        PlayerPrefs.SetInt("ThemeState" + _index, 1);

        GameManager.Instance._skinUnlockedSound.PlaySound();
    }
}
