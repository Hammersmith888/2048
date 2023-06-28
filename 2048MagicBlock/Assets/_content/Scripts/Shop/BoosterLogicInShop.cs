using UnityEngine;

public class BoosterLogicInShop : MonoBehaviour
{
    private const string DATA_KEY = "DataInfo";
    enum BoosterType
    {
        TimeBooster,
        SawBooster
    }

    [Header("Animation Expense Coins")]
    [SerializeField] private RectTransform _finalPositionCoinsAnim;
    [SerializeField] private AnimationExpenseCoins _AnimationExpenseCoins;
    [SerializeField] private float _timeAnimationPlay = 0.4f;
    [SerializeField] private float _editAnimPosFinalY = 50;
    [SerializeField] private float _editAnimPosFinalX = 50;
    [SerializeField] private int _coinPrice = 50;
    [SerializeField] private CoinPurse _coinsPurse;
    [Header("Another")]
    [SerializeField] private bool _isAdsBtn = false;
    [SerializeField] private int _countOfferBooster = 1;
    [SerializeField] private BoosterType _boosterType;

    public void BuyBooster()
    {
        if (_coinsPurse.CoinsCount >= _coinPrice && !_isAdsBtn)
        {
            _AnimationExpenseCoins.SetFinalPosition(_finalPositionCoinsAnim);
            _AnimationExpenseCoins.SetTimeToMove(_timeAnimationPlay);
            _AnimationExpenseCoins.SetEditAnimPosFinalY(_editAnimPosFinalY);
            _AnimationExpenseCoins.SetEditAnimPosFinalX(_editAnimPosFinalX);
            _AnimationExpenseCoins.StartAnimation(-_coinPrice);

            AddBoosterInGameData();

            //Invoke("SectionCodeSpeen", _AnimationExpenseCoins.GetTimeToMove() + 0.6f);
        }
        else if(_isAdsBtn)
        {

            AdsController.Instance.ShowRewardedInterstitialAd(() => {

                //_AnimationExpenseCoins.SetFinalPosition(_finalPositionCoinsAnim);
                //_AnimationExpenseCoins.SetTimeToMove(_timeAnimationPlay);
                //_AnimationExpenseCoins.SetEditAnimPosFinalY(_editAnimPosFinalY);
                //_AnimationExpenseCoins.StartAnimation(-_coinPrice, true);
                // AddBoosterInGameData();
                switch (_boosterType)
                {
                    case BoosterType.TimeBooster:
                        AddBoosterAfterAd(true);
                        break;
                    case BoosterType.SawBooster:
                        AddBoosterAfterAd(false);
                        break;
                    default:
                        break;
                }
              
            });
        }
    }

    private void AddBoosterAfterAd(bool isTimer)
    {
       GamePlayerData gamePlayerData = JsonUtility.FromJson<GamePlayerData>(PlayerPrefs.GetString(DATA_KEY));
        gamePlayerData.sawStatus = BoosterLogic.Instance.sawBooster.Enabled;
        gamePlayerData.timerStatus = BoosterLogic.Instance.timeBooster.Enabled;

        gamePlayerData.sawCount = BoosterLogic.Instance.sawBooster.Count+1;
        gamePlayerData.timerCount = BoosterLogic.Instance.timeBooster.Count+1;

        gamePlayerData.timer = CellLogic.Instance.timer.value;
        gamePlayerData.timerDelay = CellLogic.Instance.timer.delay;
        PlayerPrefs.SetString(DATA_KEY, JsonUtility.ToJson(gamePlayerData));
    }

    private void AddBoosterInGameData()
    {
        switch (_boosterType)
        {
            case BoosterType.TimeBooster:
                GameManager.Instance.AddTimer(_countOfferBooster, false);
                break;
            case BoosterType.SawBooster:
                GameManager.Instance.AddSaw(_countOfferBooster, false);
                break;
            default:
                break;
        }
    }
}
