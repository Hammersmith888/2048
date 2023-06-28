using DG.Tweening;
using UnityEngine;

public class RotationAnim : MonoBehaviour
{
    [SerializeField]
    private float rotationMin;
    [SerializeField]
    private float rotationMax;
    [SerializeField]
    private float duration = 1f;

    private Tween tween;

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        if (tween != null) return;

        tween = transform.DOLocalRotate(new Vector3(0, 0, rotationMax), duration, RotateMode.Fast);
        tween.onKill += () => {
            tween = transform.DOLocalRotate(new Vector3(0, 0, rotationMin), duration, RotateMode.Fast);
            
            tween.onKill += () => {
                tween = null; 
                if (gameObject.activeSelf && enabled)
                    Init();
            };
            
        };
    }

    private void OnDisable()
    {
        transform.DOKill();
        tween = null;
    }

}
