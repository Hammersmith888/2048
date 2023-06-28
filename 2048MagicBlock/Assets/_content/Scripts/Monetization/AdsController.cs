using System;
using System.Collections;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;


    public class AdsController : MonoBehaviour
    {
        public static AdsController Instance;
        public bool IsPurchasedRemoveADS = false;
        private const string ADS_STATE = "StateOfAds";
        private AppOpenAd _appOpenAd;
        private BannerView _bannerView;
        private InterstitialAd _interstitialAd;
        private InterstitialAd _interstitialStartAd;
        private RewardedAd _rewardedAd;
        private RewardedInterstitialAd _rewardedInterstitialAd;
        private float _deltaTime;
        private bool _isShowingAppOpenAd = false;
        private bool _isShowingBanner;
         private int timeInter = 0;
        public bool IsAdsDisabled => PlayerPrefs.GetInt(ADS_STATE, 0) == 1;
        public bool IsBannerView => _isShowingBanner;
         Action reward;
        public Action OnAdLoadedEvent;
        public Action OnAdFailedToLoadEvent;
        public Action OnAdOpeningEvent;
        public Action OnAdFailedToShowEvent;
        public Action OnUserEarnedRewardEvent;
        public Action OnAdClosedEvent;
        [SerializeField]
        private AnimationPanel LoadPopup;

        private int countOfAppOpen = 0;

        string BANNER_ID = "ca-app-pub-7814749540601690/9820876089";
        string INTER_ID ="ca-app-pub-7814749540601690/2682405052";
        string INTER_REW= "ca-app-pub-7814749540601690/8316222724";
        string OPEN_APP="ca-app-pub-7814749540601690/4376977715";
        string INTER_START="ca-app-pub-7814749540601690/7466175489";


    #region UNITY MONOBEHAVIOR METHODS
    private void Awake()
    {
        Instance = this;
    }
    public void Start()
        {
            if (PlayerPrefs.GetInt(ADS_STATE, 0) == 1)
                IsPurchasedRemoveADS = true;
            else
                IsPurchasedRemoveADS = false;

            MobileAds.SetiOSAppPauseOnBackground(true);

            // Configure TagForChildDirectedTreatment and test device IDs.
            RequestConfiguration requestConfiguration =
                new RequestConfiguration.Builder()
                    .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.False).build();
            MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(HandleInitCompleteAction);
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

        //private IEnumerator aaa()
        //{
        //    yield return new WaitForSeconds(4f);
        //    LoadPopup.Show();
        //    LoadPopup.gameObject.GetComponent<PopupSponsorAdLoad>().Show(()=> { ShowInterstitialAd(); });
        //}
        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            Debug.Log("Initialization complete.");

            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // the main thread.
            // In this example we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.

            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                // Listen to application foreground and background events.
               
                Debug.Log("Initialization complete.");
                //RequestAndLoadRewardedAd();
                //RequestAndLoadAppOpenAd();
                RequestAndLoadInterstitialAd();
                RequestAndLoadRewardedInterstitialAd();
                RequestAndLoadInterstitialStartAd();
                ShowBanner();
                StartCoroutine(TimerDelayAutoAds());

            });
        }

        #endregion

        #region HELPER METHODS

        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder().Build();
        }

    private void OnAppStateChanged(AppState state)
    {
        // Display the app open ad when the app is foregrounded.
        UnityEngine.Debug.Log("App State is " + state);
        //Debug.LogError("---!_isShowingAppOpenAd "+ !_isShowingAppOpenAd);
        if (state == AppState.Foreground && !_isShowingAppOpenAd)
        {
            if (countOfAppOpen % 2 == 0)
            {
                //Debug.LogError("ShowAppOpenAd");
                //ShowAppOpenAd();
                Debug.LogError("ShowInterstitialStartAd");
                ShowInterstitialStartAd();
            }
            else
            {
                Debug.LogError("ShowInterstitialStartAd");
                ShowInterstitialStartAd();
            }
            countOfAppOpen++;
        }
    }


    public void EnableAds()
        {
            PlayerPrefs.SetInt(ADS_STATE, 0);
            PlayerPrefs.Save();
        }

        public void DisableAds()
        {
            PlayerPrefs.SetInt(ADS_STATE, 1);
            PlayerPrefs.Save();

            DestroyBannerAd();
        }

        #endregion

        #region BANNER ADS

        public void RequestBannerAd()
        {
            PrintStatus("Requesting Banner ad.");

            // These ad units are configured to always serve test ads.

            string adUnitId = BANNER_ID;


            // Clean up banner before reusing
            DestroyBannerAd();

            // Create a 320x50 banner at top of the screen
            _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

            // Add Event Handlers
            _bannerView.OnAdLoaded += (sender, args) =>
            {
                PrintStatus("Banner ad loaded.");
            };
            _bannerView.OnAdFailedToLoad += (sender, args) =>
            {
                PrintStatus("Banner ad failed to load with error: " + args.LoadAdError.GetMessage());
            };
            _bannerView.OnAdOpening += (sender, args) =>
            {
                PrintStatus("Banner ad opening.");
            };
            _bannerView.OnAdClosed += (sender, args) =>
            {
                PrintStatus("Banner ad closed.");
            };
            _bannerView.OnPaidEvent += (sender, args) =>
            {
                string msg = string.Format("{0} (currency: {1}, value: {2}",
                    "Banner ad received a paid event.",
                    args.AdValue.CurrencyCode,
                    args.AdValue.Value);
                PrintStatus(msg);
            };

            // Load a banner ad
            _bannerView.LoadAd(CreateAdRequest());
        }

        public void DestroyBannerAd()
        {
            if (_bannerView != null)
            {
                _bannerView.Destroy();
            }
        }

        public void ShowBanner()
        {
            if (IsAdsDisabled)
            {
                DestroyBannerAd();
                return;
            }

            if (_isShowingBanner)
                _bannerView.Show();

            _isShowingBanner = true;
            RequestBannerAd();
        }

        #endregion

        #region INTERSTITIAL ADS

        public void RequestAndLoadInterstitialAd()
        {
            PrintStatus("Requesting Interstitial ad.");

 
            string adUnitId = INTER_ID;


            // Clean up interstitial before using it
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
            }

            _interstitialAd = new InterstitialAd(adUnitId);

            // Add Event Handlers
            _interstitialAd.OnAdLoaded += (sender, args) =>
            {
                PrintStatus("Interstitial ad loaded.");
                OnAdLoadedEvent?.Invoke();
            };
            _interstitialAd.OnAdFailedToLoad += (sender, args) =>
            {
                _isShowingAppOpenAd = false;
                PrintStatus("Interstitial ad failed to load with error: " + args.LoadAdError.GetMessage());
                OnAdFailedToLoadEvent?.Invoke();
            };
            _interstitialAd.OnAdOpening += (sender, args) =>
            {
                PrintStatus("Interstitial ad opening.");
                OnAdOpeningEvent?.Invoke();
            };
            _interstitialAd.OnAdClosed += (sender, args) =>
            {
                _isShowingAppOpenAd = false;
                DestroyInterstitialAd();
                RequestAndLoadInterstitialAd();
                PrintStatus("Interstitial ad closed.");
                OnAdClosedEvent?.Invoke();
                BackgroundMusicInstance.Instance.ContinueMusic();
            };
            _interstitialAd.OnAdDidRecordImpression += (sender, args) =>
            {
                PrintStatus("Interstitial ad recorded an impression.");
            };
            _interstitialAd.OnAdFailedToShow += (sender, args) =>
            {
                _isShowingAppOpenAd = false;
                PrintStatus("Interstitial ad failed to show.");
                BackgroundMusicInstance.Instance.ContinueMusic();
            };
            _interstitialAd.OnPaidEvent += (sender, args) =>
            {
                string msg = string.Format("{0} (currency: {1}, value: {2}",
                    "Interstitial ad received a paid event.",
                    args.AdValue.CurrencyCode,
                    args.AdValue.Value);
                PrintStatus(msg);
            };

            // Load an interstitial ad
            _interstitialAd.LoadAd(CreateAdRequest());
        }

        public void ShowInterstitialAd()
        {
            if (IsAdsDisabled)
                return;

            if (_interstitialAd != null && _interstitialAd.IsLoaded() && !_isShowingAppOpenAd)
            {
                BackgroundMusicInstance.Instance.PauseMusic();
            GameManager.Instance.EnableEventSystem();
            _isShowingAppOpenAd = true;
            _interstitialAd.Show();
            }
            else
            {
            _isShowingAppOpenAd = false;
            PrintStatus("Interstitial ad is not ready yet.");
                BackgroundMusicInstance.Instance.ContinueMusic();
            }
        }

        public void DestroyInterstitialAd()
        {
            if (_interstitialAd != null)
            {
            _isShowingAppOpenAd = false;
            _interstitialAd.Destroy();
            }
        }

    public void RequestAndLoadInterstitialStartAd()
    {
        PrintStatus("Requesting Interstitial ad.");


        string adUnitId = INTER_START;


        // Clean up interstitial before using it
        if (_interstitialStartAd != null)
        {
            _interstitialStartAd.Destroy();
        }

        _interstitialStartAd = new InterstitialAd(adUnitId);

        // Add Event Handlers
        _interstitialStartAd.OnAdLoaded += (sender, args) =>
        {
            PrintStatus("Interstitial ad loaded.");
            OnAdLoadedEvent?.Invoke();
        };
        _interstitialStartAd.OnAdFailedToLoad += (sender, args) =>
        {
            _isShowingAppOpenAd = false;
            PrintStatus("Interstitial ad failed to load with error: " + args.LoadAdError.GetMessage());
            OnAdFailedToLoadEvent?.Invoke();
        };
        _interstitialStartAd.OnAdOpening += (sender, args) =>
        {
            PrintStatus("Interstitial ad opening.");
            OnAdOpeningEvent?.Invoke();
        };
        _interstitialStartAd.OnAdClosed += (sender, args) =>
        {
            _isShowingAppOpenAd = false;
            DestroyInterstitialAd();
            RequestAndLoadInterstitialAd();
            RequestAndLoadInterstitialStartAd();
            PrintStatus("Interstitial ad closed.");
            OnAdClosedEvent?.Invoke();
        };
        _interstitialStartAd.OnAdDidRecordImpression += (sender, args) =>
        {
            PrintStatus("Interstitial ad recorded an impression.");
        };
        _interstitialStartAd.OnAdFailedToShow += (sender, args) =>
        {
            _isShowingAppOpenAd = false;
            PrintStatus("Interstitial ad failed to show.");
        };
        _interstitialStartAd.OnPaidEvent += (sender, args) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                "Interstitial ad received a paid event.",
                args.AdValue.CurrencyCode,
                args.AdValue.Value);
            PrintStatus(msg);
        };

        // Load an interstitial ad
        _interstitialStartAd.LoadAd(CreateAdRequest());
    }
    public void ShowInterstitialStartAd()
    {
        //if (IsAdsDisabled)
        //    return;

        if (_interstitialStartAd != null && _interstitialStartAd.IsLoaded() && !_isShowingAppOpenAd)
        {
            GameManager.Instance.EnableEventSystem();
            _isShowingAppOpenAd = true;
            _interstitialStartAd.Show();
        }
        else
        {
            _isShowingAppOpenAd = false;
            RequestAndLoadInterstitialStartAd();
            PrintStatus("Interstitial ad is not ready yet.");
        }
    }

    public void DestroyInterstitialStartAd()
    {
        if (_interstitialStartAd != null)
        {
            _interstitialStartAd.Destroy();
        }
    }

    #endregion

    #region REWARDED ADS

    private void RequestAndLoadRewardedAd(bool startFromLoad = false)
        {
            PrintStatus("Requesting Rewarded ad.");

            string adUnitId = INTER_REW;


            // create new rewarded ad instance
            _rewardedAd = new RewardedAd(adUnitId);

            if (startFromLoad)
            {
                _rewardedAd.OnAdLoaded += (er, error) =>
                {
                    //BackgroundMusicInstance.Instance.PauseMusic(true);
                    _rewardedAd.Show();
                };
            }

            _rewardedAd.OnAdOpening += (sender, e) =>
            {
                BackgroundMusicInstance.Instance.PauseMusic();
                Time.timeScale = 0;
            };

            _rewardedAd.OnAdFailedToLoad += OnRewardedFail;
            _rewardedAd.OnAdFailedToShow += OnRewardedFail;

            _rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
            _rewardedAd.OnAdClosed += (ea, error) =>
            {Debug.LogError("-----testreward1");
                Time.timeScale = 1;
                
                BackgroundMusicInstance.Instance.ContinueMusic();
            };

            // Create empty ad request
            _rewardedAd.LoadAd(CreateAdRequest());
        }

        private void UnsubscribeReward()
        {
            if (_rewardedAd != null)
            {
                _rewardedAd.OnAdFailedToLoad -= OnRewardedFail;
                _rewardedAd.OnAdFailedToShow -= OnRewardedFail;

                _rewardedAd.OnUserEarnedReward -= OnUserEarnedReward;
            }

           // RequestAndLoadRewardedAd();
        }

        private Action _rewardedCallback;
        private bool _isShowingInterAd;
        private bool _isShowingRewardedAd;

        public void ShowRewardedAd(Action callback)
        {
           

            if (_rewardedCallback != null)
                _rewardedCallback = null;

            _rewardedCallback = callback;

            if (_rewardedAd != null)
            {
            GameManager.Instance.EnableEventSystem();
            BackgroundMusicInstance.Instance.PauseMusic();
            _rewardedAd.Show();

            
            }
            else
            {
            // RequestAndLoadRewardedAd(true);
            BackgroundMusicInstance.Instance.ContinueMusic();
            PrintStatus("Rewarded ad is not ready yet.");
            }
        }

        private void OnUserEarnedReward(object sender, Reward reward)
        {
            UnsubscribeReward();

            _rewardedCallback?.Invoke();
            _rewardedCallback = null;
        }

        private void OnRewardedFail(object sender, EventArgs reward)
        {
            UnsubscribeReward();

            _rewardedCallback?.Invoke();
            _rewardedCallback = null;
        }

        public void RequestAndLoadRewardedInterstitialAd()
        {
            PrintStatus("Requesting Rewarded Interstitial ad.");

            // These ad units are configured to always serve test ads.

            string adUnitId = INTER_REW;


            // Create an interstitial.
            RewardedInterstitialAd.LoadAd(adUnitId, CreateAdRequest(), (rewardedInterstitialAd, error) =>
            {
                if (error != null)
                {
                    PrintStatus("Rewarded Interstitial ad load failed with error: " + error);
                    return;
                }

                this._rewardedInterstitialAd = rewardedInterstitialAd;
                PrintStatus("Rewarded Interstitial ad loaded.");

                // Register for ad events.
                this._rewardedInterstitialAd.OnAdDidPresentFullScreenContent += (sender, args) =>
                {

                    PrintStatus("Rewarded Interstitial ad presented.");
                    RequestAndLoadRewardedInterstitialAd();
                };
                this._rewardedInterstitialAd.OnAdDidDismissFullScreenContent += (sender, args) =>
                {
                    PrintStatus("Rewarded Interstitial ad dismissed.");
                    this._rewardedInterstitialAd = null;
                   RequestAndLoadRewardedInterstitialAd();
                };
                this._rewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += (sender, args) =>
                {
                    PrintStatus("Rewarded Interstitial ad failed to present with error: " +
                                args.AdError.GetMessage());
                    this._rewardedInterstitialAd = null;
                    RequestAndLoadRewardedInterstitialAd();
                };
                this._rewardedInterstitialAd.OnPaidEvent += (sender, args) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                        "Rewarded Interstitial ad received a paid event.",
                        args.AdValue.CurrencyCode,
                        args.AdValue.Value);
                    PrintStatus(msg);
                    RequestAndLoadRewardedInterstitialAd();
                };
                this._rewardedInterstitialAd.OnAdDidRecordImpression += (sender, args) =>
                {
                    PrintStatus("Rewarded Interstitial ad recorded an impression.");
                   
                };
            });
        }

        public void ShowRewardedInterstitialAd(Action callback)
        {
        //Debug.LogError("----ShowRewardedInterstitialAd(Action callback)");
        if (_rewardedInterstitialAd != null)
            {
                BackgroundMusicInstance.Instance.PauseMusic();
                reward = callback;
            GameManager.Instance.EnableEventSystem();
            _rewardedInterstitialAd.Show(userEarnedRewardCallback);
            }
            else
            {
                BackgroundMusicInstance.Instance.ContinueMusic();
                PrintStatus("Rewarded Interstitial ad is not ready yet.");
            }
            RequestAndLoadRewardedInterstitialAd();
    }
    private void userEarnedRewardCallback(Reward êeward)
    {
        UnityMainThreadDispatcher.Dispatcher.Enqueue(() => {
            reward?.Invoke();
            BackgroundMusicInstance.Instance.ContinueMusic();
            ResetTimeForAutoAds();
        });
    }
    private IEnumerator waiter(Action callback)
    {
        yield return new WaitForSeconds(2f);
       
    }

        #endregion

        #region APPOPEN ADS

        /*public void RequestAndLoadAppOpenAd()
        {
            PrintStatus("Requesting App Open ad.");

            string adUnitId = OPEN_APP;

            // create new app open ad instance
            AppOpenAd.LoadAd(adUnitId, ScreenOrientation.Portrait, CreateAdRequest(), (appOpenAd, error) =>
            {
                if (error != null)
                {
                    PrintStatus("App Open ad failed to load with error: " + error);
                    return;
                }

                PrintStatus("App Open ad loaded. Please background the app and return.");
                this._appOpenAd = appOpenAd;
            });
        }*/
        /*
        public void ShowAppOpenAd()
        {
            //if (IsAdsDisabled)
            //    return;

            //if (_isShowingAppOpenAd)
            //    return;

            if (_appOpenAd == null)
                return;

            //if (_isShowingRewardedAd)
            //{
            //    //if (!RemoteController.isNeedAppOpenAdAfterReward)
            //    //{
            //    //    _isShowingInterAd = false;
            //    //    _isShowingRewardedAd = false;
            //    //    return;
            //    //}

            //}


            //if (_isShowingInterAd)
            //{
            //    //if (!RemoteController.isNeedAppOpenAdAfterInter)
            //    //{
            //    //    _isShowingInterAd = false;
            //    //    _isShowingRewardedAd = false;
            //    //    return;
            //    //}

            //}
            //_isShowingInterAd = false;
            //_isShowingRewardedAd = false;

            // Register for ad events.
            this._appOpenAd.OnAdDidDismissFullScreenContent += (sender, args) =>
            {
                PrintStatus("App Open ad dismissed.");
                _isShowingAppOpenAd = false;
                MobileAdsEventExecutor.ExecuteInUpdate(() => {
                    if (this._appOpenAd != null)
                    {
                        this._appOpenAd.Destroy();
                        this._appOpenAd = null;
                    }
                });
            };
            this._appOpenAd.OnAdFailedToPresentFullScreenContent += (sender, args) =>
            {
                PrintStatus("App Open ad failed to present with error: " + args.AdError.GetMessage());

                _isShowingAppOpenAd = false;
                MobileAdsEventExecutor.ExecuteInUpdate(() => {
                    if (this._appOpenAd != null)
                    {
                        this._appOpenAd.Destroy();
                        this._appOpenAd = null;
                    }
                });
            };
            this._appOpenAd.OnAdDidPresentFullScreenContent += (sender, args) =>
            {
                PrintStatus("App Open ad opened.");
                _isShowingAppOpenAd = true;
            };
            this._appOpenAd.OnAdDidRecordImpression += (sender, args) =>
            {
                PrintStatus("App Open ad recorded an impression.");
            };
            this._appOpenAd.OnPaidEvent += (sender, args) =>
            {
                string msg = string.Format("{0} (currency: {1}, value: {2}",
                    "App Open ad received a paid event.",
                    args.AdValue.CurrencyCode,
                    args.AdValue.Value);
                PrintStatus(msg);
            };
            _appOpenAd.Show();
            StartCoroutine(LoadAppOpenWithDelay());
        }
    */
    private IEnumerator LoadAppOpenWithDelay()
    {
        yield return new WaitForSeconds(1f);
        //RequestAndLoadAppOpenAd();
        RequestAndLoadInterstitialStartAd();
        
    }

    #endregion


    #region AD INSPECTOR

    public void OpenAdInspector()
        {
            PrintStatus("Open ad Inspector.");

            MobileAds.OpenAdInspector((error) =>
            {
                if (error != null)
                {
                    PrintStatus("ad Inspector failed to open with error: " + error);
                }
                else
                {
                    PrintStatus("Ad Inspector opened successfully.");
                }
            });
        }

        #endregion

        #region Utility

        ///<summary>
        /// Log the message and update the status text on the main thread.
        ///<summary>
        private void PrintStatus(string message)
        {
            Debug.Log(message);
            MobileAdsEventExecutor.ExecuteInUpdate(() => {
                Debug.Log(message);
            });
        }
    private IEnumerator TimerDelayAutoAds()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);

            if (GameManager.Instance.CurrentScreen == GameManager.GameScreen.Game
                && GameManager.Instance.isPause == false)
            {
                timeInter += 1;
            }

            if (timeInter > RemoteController.Instance.timeForAd 
                && GameManager.Instance.CurrentScreen == GameManager.GameScreen.Game
                && GameManager.Instance.isPause == false)
            {
                timeInter = 0;
                if (_interstitialAd.IsLoaded())
                {

                    LoadPopup.Show();
                    LoadPopup.gameObject.GetComponent<PopupSponsorAdLoad>().Show(()=> { 
                        ShowInterstitialAd(); 
                    });

                }
                else
                {
                    timeInter = 0;
                    RequestAndLoadInterstitialAd();
                }

            }
        }
    }

    public void ResetTimeForAutoAds()
    {
        timeInter = 0;
    }
    #endregion
}
