using DG.Tweening;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    [SerializeField]
    private float scale = 0.5f;
    [SerializeField]
    private float duration = 1f;

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        transform.DOScale(scale, duration).SetLoops(-1, LoopType.Yoyo);       
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
