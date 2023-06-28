using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

public class MoneyStats : MonoBehaviour
{
    /// переделать на ивенты?
    
    private int money;
    private Canvas _canvas;

    [SerializeField]
    private TextMeshProUGUI text;

    private void Start()
    {
        _canvas=GetComponent<Canvas>();
        UpdateMoney();
        UpdateUI();
    }

    public void SetOrderInLayer(int number)
    {
        if (_canvas != null)
        {
            _canvas.sortingOrder = number;
        }
    }
    public void AddMoneyWithAnimation(int count)
    {
        Debug.Log(money);
        Debug.Log(money + count);
        DOTween.To(() => money, x => money = x, money + count, 1.5f).OnComplete(()=> { GameManager.Instance.AddMoney(count);});
    }
  
    private void Update()
    {
        
        
        if(money != CoinPurse.Instance.CoinsCount)
        {
            UpdateMoney();
            UpdateUI();
        }
    }

    private void UpdateMoney()
    {
        money = CoinPurse.Instance.CoinsCount;
    }

    private void UpdateUI()
    {
        text.text = money.ToString();
    }
}
