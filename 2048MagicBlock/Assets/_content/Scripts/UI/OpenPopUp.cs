using UnityEngine;
using DG.Tweening;

public class OpenPopUp : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    public void OpenPopup()
    {
        _canvasGroup.DOFade(1, 0.5f);
    }

    public void ClosePopup()
    {
        _canvasGroup.DOFade(0, 0.5f).onComplete += () =>
        {
            gameObject.SetActive(false);
        };
    }
}
