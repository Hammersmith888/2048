using UnityEngine;

public class OnEnableActiveBlackBG : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.ShowBlackBG();
    }

    private void OnDisable()
    {
        GameManager.Instance.HideBlackBG();
    }
}
