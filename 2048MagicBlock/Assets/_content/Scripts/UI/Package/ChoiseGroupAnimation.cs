using UnityEngine;
using DG.Tweening;

public class ChoiseGroupAnimation : MonoBehaviour
{
    [SerializeField] private CanvasGroup _active, _notActive;
    [SerializeField] private Transform _textTransform;


    public void Active()
    {
        _active.DOFade(1, 0.5f);
        _notActive.DOFade(0, 0.5f);
        _textTransform.DOLocalMoveY(6, 0.5f);
        gameObject.transform.DOLocalMoveY(0, 0.5f);
    }

    public void NotActive()
    {
        _active.DOFade(0, 0.5f);
        _notActive.DOFade(1, 0.5f);
        _textTransform.DOLocalMoveY(17, 0.5f);
        gameObject.transform.DOLocalMoveY(-25, 0.5f);
    }
}
