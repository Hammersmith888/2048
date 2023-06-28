using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RewardPanelAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _topPlate;

    [SerializeField] private Transform _startPosition, _endPosition;

    [SerializeField] private Image _chestsBar;

    private Transform _plate;

    private CanvasGroup _canvasGroup;

    private float _fullFill = 30;

    private int _currentFill;

    public void Awake()
    {
        _plate = _topPlate.transform;
        _canvasGroup = _topPlate.GetComponent<CanvasGroup>();
        _plate.DOMoveY(_startPosition.position.y, 0f);
        _canvasGroup.alpha = 0;

        LoadCurrentFill();

        _chestsBar.fillAmount = SubtractionOfValues(_currentFill); 
    }

    private void LoadCurrentFill()
    {
        if (!PlayerPrefs.HasKey("CurrentFill"))
        {
            PlayerPrefs.SetInt("CurrentFill", 0);
            _currentFill = 0;
        }

        else
        {
            _currentFill = PlayerPrefs.GetInt("CurrentFill");
        }
    }

    private void SaveCurrentFill()
    {
        PlayerPrefs.SetInt("CurrentFill", _currentFill);
    }

    private float SubtractionOfValues(float current)
    {
        return current / _fullFill;
    }


    public void UPFill()
    {
        _currentFill++;
        _chestsBar.fillAmount = SubtractionOfValues(_currentFill);
        SaveCurrentFill();
    }

    public void ShowRewardPanel()
    {
        _canvasGroup.DOFade(1, 0.5f).SetDelay(0.5f);
        _plate.DOMoveY(_endPosition.position.y, 0.5f).SetDelay(0.5f);
    }

    public void HideRewardPanel()
    {
        _canvasGroup.DOFade(0, 0.5f);
        _plate.DOMoveY(_startPosition.position.y, 0.5f);
    }
}
