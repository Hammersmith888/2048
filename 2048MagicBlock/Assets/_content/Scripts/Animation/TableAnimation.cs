using DG.Tweening;
using UnityEngine;

public class TableAnimation : MonoBehaviour
{
    [SerializeField]
    private float scale;
    [SerializeField]
    private float duration;

    [SerializeField] AnimationCurve _curve;

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        transform.DOScale(scale, duration).SetLoops(-1, LoopType.Yoyo).SetEase(_curve);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
