using UnityEngine;

public class TapToPlay : MonoBehaviour
{
    private bool noResetFlag = false;

    public void Show(bool noResetFlag = false)
    {
        if (PopupController.Instance.activePopup != null) return;

        if (!GameData.isTutorialShowedUp) return;

        GameManager.Instance.isPause = true;
        gameObject.SetActive(true);

        this.noResetFlag = noResetFlag;
    }

    public void Hide()
    {
        if (!GameData.isTutorialShowedUp)
        {
            CellLogic.Instance.ResetTimer(!noResetFlag);
        }
        else
        {
            GameManager.Instance.SetActiveUI(true);
        }

        GameManager.Instance.gameData.SaveData();
        GameManager.Instance.isPause = false;
        gameObject.SetActive(false);
    }
}

