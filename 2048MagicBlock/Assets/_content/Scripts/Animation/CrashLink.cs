using UnityEngine;
using DG.Tweening;

public class CrashLink : MonoBehaviour
{
    [SerializeField] private Transform _piece1, _piece2, _piece3, _piece4;

    [SerializeField] private Transform _endpospiece1, _endpospiece2, _endpospiece3, _endpospiece4;

    private CanvasGroup _pieceAll;



    private void Awake()
    {
        _pieceAll = gameObject.GetComponent<CanvasGroup>();
        MovePiese();
    }

    private void MovePiese()
    {
        _piece1.DOMove(_endpospiece1.position, 0.5f);
        _piece2.DOMove(_endpospiece2.position, 0.5f);
        _piece3.DOMove(_endpospiece3.position, 0.5f);
        _piece4.DOMove(_endpospiece4.position, 0.5f);

        _piece1.DOScale(0.4f, 0.5f);
        _piece2.DOScale(0.4f, 0.5f);
        _piece3.DOScale(0.4f, 0.5f);
        _piece4.DOScale(0.4f, 0.5f);

        _pieceAll.DOFade(0, 0.2f).SetDelay(0.3f);

        Destroy(gameObject, 0.6f);
    }
}
