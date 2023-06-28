using UnityEngine;
using UnityEngine.UI;

public class ShopCoinsAnimationLogic : MonoBehaviour
{
    [HideInInspector]
    public static ShopCoinsAnimationLogic Instance;

    [SerializeField] Button _specialOfferButton, _masterBundleButton, _superBundleButton;
    [Space(10)]
    [SerializeField] Button _1500Coins, _700Coins, _300Coins, _100Coins;

    [SerializeField] AnimationCollectCoins _animationCollectCoins;
    [SerializeField] AnimationCollectCoins _animationCollectCoinsSpecialOffer;

    [SerializeField] RectTransform _specialOffer, _masterBundle, _superBundle, _1500, _700, _300, _100;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        /*
        _specialOfferButton.onClick.AddListener(() => GetCoins(30000, _specialOffer));
        _masterBundleButton.onClick.AddListener(() => GetCoins(1500, _masterBundle));
        _superBundleButton.onClick.AddListener(() => GetCoins(9000, _superBundle));
        _1500Coins.onClick.AddListener(() => GetCoins(1500, _1500));
        _700Coins.onClick.AddListener(() => GetCoins(700, _700));
        _300Coins.onClick.AddListener(() => GetCoins(300, _300));
        _100Coins.onClick.AddListener(() => GetCoins(100, _100));
        */
    }

    public void GetSpecialOffer() 
    {
        if (_animationCollectCoinsSpecialOffer.gameObject.transform.parent.gameObject.activeSelf)
        {
            GetCoinsSpecialOfferPopUp(700, _specialOffer);
            GameManager.Instance.AddTimer(7,false);
            GameManager.Instance.AddSaw(7, false);
        }
        else
        {
            GetCoins(700, _specialOffer);
            GameManager.Instance.AddTimer(7, false);
            GameManager.Instance.AddSaw(7, false);
        }
        GameManager.Instance._specialOfferSound.PlaySound();
    }
    public void GetMasterOffer()
    {
        GetCoins(1500, _masterBundle);
        GameManager.Instance.AddTimer(10, false);
        GameManager.Instance.AddSaw(10, false);
    }
    public void GetSuperOffer()
    {
        GetCoins(9000, _superBundle);
        GameManager.Instance.AddTimer(50, false);
        GameManager.Instance.AddSaw(50, false);
    }
    public void Get1500Coins()
    {
        GetCoins(1500, _1500);
    }
    public void Get700Coins()
    {
        GetCoins(700, _700);
    }
    public void Get300Coins()
    {
        GetCoins(300, _300);
    }
    public void Get100Coins()
    {
        GetCoins(100, _100);
    }

    private void GetCoins(int coinsCount, RectTransform coins)
    {
        _animationCollectCoins.SetStartPosition(coins);
        _animationCollectCoins.StartAnimation(coinsCount);
    }

    private void GetCoinsSpecialOfferPopUp(int coinsCount, RectTransform coins)
    {
        _animationCollectCoinsSpecialOffer.SetStartPosition(coins);
        _animationCollectCoinsSpecialOffer.StartAnimation(coinsCount);
    }
}
