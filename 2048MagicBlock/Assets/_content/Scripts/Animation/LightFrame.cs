using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightFrame : MonoBehaviour
{
    private CanvasGroup _canvas;

    void Awake()
    {
        _canvas = gameObject.GetComponent<CanvasGroup>();
        Light();
    }

    private void Light()
    {
        _canvas.DOFade(0.5f, 1).onComplete += () => 
        {
            _canvas.DOFade(1f, 1).onComplete += () =>
            {
                Light();
            };
        };
    }
}
