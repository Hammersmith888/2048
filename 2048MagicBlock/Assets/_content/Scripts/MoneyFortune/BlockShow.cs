using UnityEngine;
using UnityEngine.UI;

public class BlockShow : MonoBehaviour
{
    [SerializeField] private Image _blockImage;
    [SerializeField] private Sprite[] _blockNumbers;

    public void ShowImage(int index)
    {
        GameManager.Instance._specialBlockSound.PlaySound();
        switch (index)
        {
            case 10:
                AnalyticsManager.Instance.LogEvent("great_block10");
                _blockImage.sprite = _blockNumbers[0];
                break;
            case 15:
                AnalyticsManager.Instance.LogEvent("great_block15");
                _blockImage.sprite = _blockNumbers[1];
                break;
            case 20:
                AnalyticsManager.Instance.LogEvent("great_block20");
                _blockImage.sprite = _blockNumbers[2];
                break;
            case 25:
                AnalyticsManager.Instance.LogEvent("great_block25");
                _blockImage.sprite = _blockNumbers[3];
                break;
        }
        Debug.Log(index + "Image cube");
    }
}
