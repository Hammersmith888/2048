using UnityEngine;
using UnityEngine.Purchasing;

public class INAPcontroller : MonoBehaviour
{
    [HideInInspector] public const string SPECIAL_OFFER = "com.oriplay.project2048.specialoffer";
    [HideInInspector] public const string MASTER_BUNDLE = "com.oriplay.project2048.masterbundle";
    [HideInInspector] public const string SUPER_BUNDLE = "com.oriplay.project2048.superbundle";
    [HideInInspector] public const string REMOVE_ADS = "com.oriplay.project2048.removeads";
    [HideInInspector] public const string COINS_1500 = "com.oriplay.project2048.1500coins";
    [HideInInspector] public const string COINS_700 = "com.oriplay.project2048.700coins";
    [HideInInspector] public const string COINS_300 = "com.oriplay.project2048.300coins";
    [HideInInspector] public const string COINS_100 = "com.oriplay.project2048.100coins";
    [HideInInspector] public const string WEEKLY_SUBSCRIPTION = "vip_weekly";
    [HideInInspector] public const string MONTHLY_SUBSCRIPTION = "vip_monthly";
    [HideInInspector] public const string YEARLY_SUBSCRIPTION = "vip_yearly";

    [SerializeField] private GameObject _removeAdsObj;

    [HideInInspector] public INAPcontroller Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (AdsController.Instance.IsAdsDisabled)
            _removeAdsObj.SetActive(false);

    }

    public void OnPurchaseCompleted1(Product product)
    {
        switch (product.definition.id)
        {
            case SPECIAL_OFFER:
                ShopCoinsAnimationLogic.Instance.GetSpecialOffer();
                break;
            case MASTER_BUNDLE:
                ShopCoinsAnimationLogic.Instance.GetMasterOffer();
                break;
            case SUPER_BUNDLE:
                ShopCoinsAnimationLogic.Instance.GetSuperOffer();
                break;
            case REMOVE_ADS:
                AdsController.Instance.DisableAds();
                _removeAdsObj.SetActive(false);
                break;
            case COINS_1500:
                ShopCoinsAnimationLogic.Instance.Get1500Coins();
                break;
            case COINS_700:
                ShopCoinsAnimationLogic.Instance.Get700Coins();
                break;
            case COINS_300:
                ShopCoinsAnimationLogic.Instance.Get300Coins();
                break;
            case COINS_100:
                ShopCoinsAnimationLogic.Instance.Get100Coins();
                break;
            case WEEKLY_SUBSCRIPTION:
                //ShopCoinsAnimationLogic.Instance.Get100Coins();
                break;
            case MONTHLY_SUBSCRIPTION:
                //ShopCoinsAnimationLogic.Instance.Get100Coins();
                break;
            case YEARLY_SUBSCRIPTION:
                //ShopCoinsAnimationLogic.Instance.Get100Coins();
                break;
            default:
                break;
        }
    }
}
