using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ButtonData
{
    public Button button;
    public int cost;
    public int value;
}

public class StartFrom : AnimationPanel
{
    public ButtonData Button5;
    public ButtonData Button10;
    public ButtonData Button18;

    private ButtonData selected;

    public GameObject[] hideObjs;

    public Button ApplyButton;

    private void Start()
    {
        Button5.button.onClick.AddListener(() => { Select(Button5); });
        Button10.button.onClick.AddListener(() => { Select(Button10); });
        Button18.button.onClick.AddListener(() => { Select(Button18); });

        ApplyButton.onClick.AddListener(() => {
            GameManager.Instance.AddMoney(-selected.cost);
            CellLogic.Instance.SetStartData(selected.value);
            GameManager.Instance.isPause = false;
            BoosterLogic.Instance.sawBooster.Enabled = true;
            BoosterLogic.Instance.timeBooster.Enabled = true;
        });

        Select(Button5);
    }

    private void OnEnable()
    {
        Button5.button.gameObject.SetActive(true);
        Button10.button.gameObject.SetActive(GameData._10Opened);
        Button18.button.gameObject.SetActive(GameData._18Opened);

        for (int i = 0; i < hideObjs.Length; i++)
        {
            hideObjs[i].SetActive(false);
        }
    }

    public void Select(ButtonData data)
    {
        ApplyButton.interactable = data.cost <= CoinPurse.Instance.CoinsCount;
        ApplyButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = data.cost.ToString();

        selected = data;

        for (int i = 0; i < hideObjs.Length; i++)
        {
            hideObjs[i].SetActive(true);
        }
    }
}


