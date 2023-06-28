using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EffectsBlock : MonoBehaviour
{
    [SerializeField] private GameObject _buyButton, _blockCanvas;

    private Button _button;

    private CanvasGroup _buyButtonCanvas, _blockCanvasGroup;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _buyButtonCanvas = _buyButton.GetComponent<CanvasGroup>();
        _blockCanvasGroup = _blockCanvas.GetComponent<CanvasGroup>();

        CloseBlock();
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
        _buyButton.SetActive(false);
        _button.interactable = false;
    }
    public void CanBuyBlock()
    {
        _buyButton.SetActive(true);
        _buyButtonCanvas.DOFade(1, 0.5f);
    }
}
