using TMPro;
using UnityEngine;

public class PackagePanel : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public GameObject moneyIcon;

    private int cost;

    private void OnEnable()
    {
        buttonText.text = "3000";
        cost = 3000;
        moneyIcon.SetActive(true);
    }

    public void SetFree()
    {
        buttonText.text = "Get";
        cost = 0;
        moneyIcon.SetActive(false);
    }

    public void Get()
    {
        if(cost <= CoinPurse.Instance.CoinsCount)
        {
            CoinPurse.Instance.CostSubtraction(cost);
            GetComponent<PopupShow>().HidePopup();
        }

       // gameObject.SetActive(false);
    }
}

