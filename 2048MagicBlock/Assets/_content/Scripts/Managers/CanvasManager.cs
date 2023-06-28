using UnityEngine;

public class CanvasManager : MonoBehaviourSingleton<CanvasManager>
{
    public static float Height => Instance.rectTransform.rect.height;
    public static float Width => Instance.rectTransform.rect.width;

    private RectTransform rectTransform {
        get
        {
            if(_rectTransform == null) 
                _rectTransform = GetComponent<RectTransform>();

            return _rectTransform;
        }
    }
    private RectTransform _rectTransform;
}

