using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ThemesBlock : MonoBehaviour
{
    [SerializeField] private GameObject _buyButton, _blockCanvas;
    [SerializeField] private int _cost;
    
    private Button _button;

    private CanvasGroup _buyButtonCanvas, _blockCanvasGroup;

    public ThemeType type;

    private bool _state;
    private int _intState;

    private void Awake()
    {

        _button = GetComponent<Button>();

        _buyButtonCanvas = _buyButton.GetComponent<CanvasGroup>();
        _blockCanvasGroup = _blockCanvas.GetComponent<CanvasGroup>();

        LoadState();

        _button.onClick.AddListener(() => {
                ThemeController.Instance.currentTheme = type;
                ThemeController.Instance.ApplyTheme();
        });
    }

    public void OpenBlock()
    {
        _blockCanvasGroup.DOFade(0, 0.5f).onComplete += () => 
        {
            _blockCanvas.SetActive(false);
            _button.interactable = true;
        };
    }

    public void CloseBlock()
    {
        _blockCanvasGroup.alpha = 1;
        _buyButtonCanvas.alpha = 1;
        _blockCanvas.SetActive(true);
        _buyButton.SetActive(true);
        _button.interactable = false;
    }

    public void BuySkin()
    {
        if (_cost >= CoinPurse.Instance.CoinsCount)
        {
            CoinPurse.Instance.CostSubtraction(_cost);

            OpenBlock();

            PlayerPrefs.SetInt("ThemeState" + _cost, 1);
            _state = true;
        }
    }

    private void LoadState()
    {
        if (_cost > 0)
        {
            if (!PlayerPrefs.HasKey("ThemeState" + _cost))
            {
                PlayerPrefs.SetInt("ThemeState" + _cost, 0);
                _intState = 0;
            }
            else
            {
                _intState = PlayerPrefs.GetInt("ThemeState" + _cost);
            }

            if (_intState == 0)
                _state = false;
            else
                _state = true;


            if (_state == false)
                CloseBlock();
            else
                OpenBlock();
        }

        OpenBlock();
    }    
}
