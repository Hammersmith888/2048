using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class FreeCoins : MonoBehaviour
{
    [SerializeField] Button _freeCoinsReward;
    [SerializeField] int _coinsCount;
    [SerializeField] AnimationCollectCoins _animationCollectCoins;

    private RectTransform _buttonTransform;

    private void Awake()
    {
        _buttonTransform = _freeCoinsReward.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
       // _freeCoinsReward.onClick.AddListener(GetFreeCoins);
    }

    public void GetFreeCoins()
    {
        AdsController.Instance.ShowRewardedInterstitialAd(() => {
            AnalyticsManager.Instance.LogEvent("ad_watch");
            if (!GameManager.Instance.CurrentScreen.Equals(GameScreen.Game))
            {
                _animationCollectCoins.SetStartPosition(_buttonTransform);
                _animationCollectCoins.StartAnimation(_coinsCount);
            }

            if (GameManager.Instance.CurrentScreen.Equals(GameScreen.Game))
            {
                GameManager.Instance.ShowBlackBG();
            }
        });
    }
}
