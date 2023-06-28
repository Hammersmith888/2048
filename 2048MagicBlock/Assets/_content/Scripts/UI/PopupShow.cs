using UnityEngine;

public class PopupShow : MonoBehaviour
{
    private AnimationPanel _animationCache;

    private AnimationPanel _animation
    {
        get
        {
            if (_animationCache == null)
                _animationCache = GetComponent<AnimationPanel>();
            return _animationCache;
        }
    }


    public void ShowPopup()
    {
        PopupController.Instance.ShowPopup(_animation);
    }
    public void HidePopup()
    {
        PopupController.Instance.HidePopup(_animation);
    }
}
