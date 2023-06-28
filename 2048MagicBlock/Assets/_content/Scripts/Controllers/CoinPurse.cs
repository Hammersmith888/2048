using UnityEngine;

public class CoinPurse : MonoBehaviourSingleton<CoinPurse>
{
    [SerializeField] private CoinsCounter _numberCounter;

    private int _coins;
    public int CoinsCount => _coins;

    private void Start()
    {
        _numberCounter = GetComponent<CoinsCounter>();
        _coins = LoadCoins();
        _numberCounter.StartCoinsCount(_coins);
    }

    private int LoadCoins()
    {
        int coins;

        if (!PlayerPrefs.HasKey("Coins"))
        {
            PlayerPrefs.SetInt("Coins", 0);
            coins = 0;
        }
        else
        {
            coins = PlayerPrefs.GetInt("Coins");
        }
        
        return coins;
    }

    public void SaveCoins(int coins)
    {
        PlayerPrefs.SetInt("Coins", coins);
    }

    public bool CostSubtraction(int cost, bool showFreeCoinPanel = true)
    {
        if (_coins < cost)
        {
            //GameManager.Instance.OpenFreeCoins(cost - _coins);
            if (showFreeCoinPanel)
            {
                GameManager.Instance.OpenFreeCoins(30);
            }
            else
            {
                //GameManager.Instance.RevivePopup.Hide();
                GameManager.Instance.OpenShop();
            }
            
            return false;
        }

        if (_coins - cost >= 0)
            _coins -= cost;

        SaveCoins(_coins);

        SetValue(_coins);

        return true;
    }

    private void SetValue(int count)
    {
        _numberCounter.Value = count;
    }

    public void IncreaseCoins(int coins)
    {
        if (_coins + coins >= 0)
            _coins += coins;

        SaveCoins(_coins);
        SetValue(_coins);
    }

    public void ChangeCoinsValueForPackageSystem(int coins)
    {
        IncreaseCoins(coins);
        _numberCounter.StartCoinsCount(_coins);
    }
}
