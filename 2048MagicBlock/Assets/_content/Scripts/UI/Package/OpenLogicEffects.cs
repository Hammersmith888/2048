using System.Collections.Generic;
using UnityEngine;

public class OpenLogicEffects : MonoBehaviour
{
    [SerializeField] private List<EffectsBlock> _themesBlocks;

    private int _openBlockIndex;



    private void Start()
    {
        LoadData();
        OpenBlock();

        Debug.Log(CoinPurse.Instance.CoinsCount);
    }

    public void OpenBlock()
    {
        for (int i = 0; i < _openBlockIndex; i++)
        {
            _themesBlocks[i].OpenBlock();
            if (i < _themesBlocks.Count)
            {
                _themesBlocks[i + 1].CanBuyBlock();
            }
        }
    }

    private void LoadData()
    {
        if (!PlayerPrefs.HasKey("OpenBlockIndexEffects"))
        {
            PlayerPrefs.SetInt("OpenBlockIndexEffects", 1);
            _openBlockIndex = 1;
        }
        else
        {
            _openBlockIndex = PlayerPrefs.GetInt("OpenBlockIndexEffects");
        }
    }

    public void BuySkin(int cost)
    {
        Debug.Log("Buy start");
        if (CoinPurse.Instance.CoinsCount >= cost)
        {
            CoinPurse.Instance.CostSubtraction(cost);

            _openBlockIndex++;
            _themesBlocks[_openBlockIndex - 1].OpenBlock();
            _themesBlocks[_openBlockIndex].CanBuyBlock();
            Debug.Log("Good! You buy effects " + _openBlockIndex);

            PlayerPrefs.SetInt("OpenBlockIndexEffects", _openBlockIndex);
        }
    }
}
