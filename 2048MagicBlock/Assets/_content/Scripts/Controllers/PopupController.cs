using UnityEngine;

public class PopupController : MonoBehaviourSingleton<PopupController>
{
    [SerializeField] private AnimationPanel _sawBooster;

    public AnimationPanel activePopup;

    public void ShowPopup(AnimationPanel popup)
    {
        GameManager.Instance.ShowBlackBG();

        var popupCanvas = popup.GetComponent<Canvas>();
        if(popupCanvas != null)
            GameManager.Instance.SetBlackBGOrder(popupCanvas.sortingOrder);

        popup.Show();
    }

    public void ShowPopupVIP(AnimationPanel popup)
    {
        GameManager.Instance.ShowBlackBG();

        var popupCanvas = popup.GetComponent<Canvas>();
        if (popupCanvas != null)
            GameManager.Instance.SetBlackBGOrder(popupCanvas.sortingOrder);

        popup.ShowVIP();
    }

    public void HidePopup(AnimationPanel popup, bool isVip = false)
    {
        if(!isVip || GameManager.Instance.CurrentScreen != GameManager.GameScreen.Game)
            GameManager.Instance.HideBlackBG();

        var popupCanvas = popup.GetComponent<Canvas>();
        if (popupCanvas != null)
            GameManager.Instance.ResetBlackBGOrder();

        popup.Hide();
    }
}
