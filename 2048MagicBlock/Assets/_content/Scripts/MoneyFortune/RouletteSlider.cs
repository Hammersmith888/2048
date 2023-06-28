using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RouletteSlider : MonoBehaviour
{
    [SerializeField] private TMP_Text _factorNumber;
    [SerializeField] private Animator _animator;
    [SerializeField] private Button _takeBonusBtn;
    [SerializeField] private Button _takeGiftBtn;

    private int _factor;
    private int _number = 10;

    private bool _state;

    private void Start()
    {
        SetFactor();
    }

    public void SetDouble()
    {
        _factor = 2;
        SetFactor();
    }

    public void SetTriple()
    {
        _factor = 3;
        SetFactor();
    }
    public void SetQuad()
    {
        _factor = 4;
        SetFactor();
    }

    public void SetFive()
    {
        _factor = 5;
        SetFactor();
    }

    public void DisableFactor()
    {
        _animator.enabled = false;
        _takeBonusBtn.interactable = false;
        _takeGiftBtn.interactable = false;
    }

    // Получение монет за уровень
    public void IncreaseCoinsGetButton()
    {
        CoinPurse.Instance.IncreaseCoins(_number);
    }

    private void SetFactor()
    {
      //  Debug.Log(_factor);
        _factorNumber.text = (_number * _factor).ToString();
    }

    // Получение монет за уровень с множителем
    public void IncreaseCoins()
    {
        AdsController.Instance.ShowRewardedInterstitialAd(() => {
            if (_state == false)
            {
                CoinPurse.Instance.IncreaseCoins(_number * _factor);

                _state = true;
                Debug.Log("IncreaseCoins");
            }
        });
        
    }
}