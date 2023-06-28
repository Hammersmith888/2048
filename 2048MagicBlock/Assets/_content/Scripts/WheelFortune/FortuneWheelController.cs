using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class FortuneWheelController : MonoBehaviourSingleton<FortuneWheelController>
{
    [SerializeField] private string[] _prizeText;
    [SerializeField] private CanvasGroup _choiseFrame, _shinningReward, _shinningRewardSecond;
    [SerializeField] private GameObject _rewardWindow;
    [SerializeField] private GameObject _animEffectWheelObj1;
    [SerializeField] private GameObject _animEffectWheelObj2;
    [SerializeField] private GameObject _animEffectWheelObj3;
    [SerializeField] private List<GameObject> _rewardImage;
    [SerializeField] private Button _coinsButton, _rewardButton, _okButtonTakeGift;
    [SerializeField] private RewardPanelAnimation _rewardPanelAnimation;
    [SerializeField] private int _speenCost;
    [SerializeField] private Transform _wheel;
    [SerializeField] private float _speed;
    [SerializeField] AnimationCurve _curve, _loopCurve, _loopCurveRetry;
    [SerializeField] AnimationCollectCoins _animationCollectCoins;
    [Header("")]
    [SerializeField] private RectTransform _finalPositionCoinsAnim;
    [SerializeField] private AnimationExpenseCoins _AnimationExpenseCoins;
    [SerializeField] private float _timeAnimationPlay = 0.4f;
    [SerializeField] private float _editAnimPosFinalY = 50;
    [Header("")]
    [SerializeField] private CoinPurse _coinsPurse;
    [SerializeField] private RewardPanelAnimation _RewardPanelAnimation;

    private int _indexReward;
    private CanvasGroup _rewardCanvasGroup;
    private CanvasGroup _coinsButtonCanvas, _rewardButtonCanvas;
    private Tween _tween;
    private bool _state;
    private bool _canLoopSpeenRetry;
    private int _random;

    private void Start()
    {
        _coinsButtonCanvas = _coinsButton.GetComponent<CanvasGroup>();
        _rewardButtonCanvas = _rewardButton.GetComponent<CanvasGroup>();

        _rewardCanvasGroup = _rewardWindow.GetComponent<CanvasGroup>();

        _choiseFrame.alpha = 0;
        _shinningReward.alpha = 0;
        _shinningRewardSecond.alpha = 0;

        _rewardCanvasGroup.alpha = 0;

        _rewardWindow.SetActive(false);

        _coinsButton.enabled = true;
        _rewardButton.enabled = true;
    }

    #region Wheel fortune logic
    public void LoopSpeen()
    {
        _coinsButton.enabled = false;
        _rewardButton.enabled = false;

        StartCoroutine(ShowButton());

        _state = false;
        _tween.Kill();
        _tween = _wheel.DORotate(new Vector3(0, 0, -(350 * 2)), _speed / 2).SetEase(_loopCurve);
        _tween.onComplete += () =>
        {
            LoopSpeenRetry();
        };
    }

    private void LoopSpeenRetry()
    {

        _tween = _wheel.DORotate(new Vector3(0, 0, -(350 * 6)), _speed * 1f).SetEase(_loopCurveRetry);
        _tween.onComplete += () =>
        {
            LoopSpeenRetry();
        };
    }

    public void SpeenByMoney()
    {
        if (CoinPurse.Instance.CoinsCount >= _speenCost)
            Speen(false);

        CoinPurse.Instance.CostSubtraction(_speenCost);
    }

    public event System.Action<float> wheelSpeen;
    public event System.Action wheelEnd;

    public void Speen(bool isAd)
    {
        if (isAd){
            AdsController.Instance.ShowRewardedInterstitialAd(()=>{
                GameManager.Instance.AddFreeSpin();
                SpeenWheel(); 
            });
        }
        else
        {
            SpeenWheel();
        }
        void SpeenWheel() 
        {
            GameManager.Instance.DisableEventSystem();

            if (GameManager.Instance.gameData.freeSpeenCount > 0)
            {
                AnalyticsManager.Instance.LogEvent("wheel_free");
                GameManager.Instance.Del1FreeSpin();

                SectionCodeSpeen();
            }
            else
            {
                if (_coinsPurse.CoinsCount >= _speenCost)
                {
                    AnalyticsManager.Instance.LogEvent("wheel_coins");
                    _AnimationExpenseCoins.SetFinalPosition(_finalPositionCoinsAnim);
                    _AnimationExpenseCoins.SetTimeToMove(_timeAnimationPlay);
                    _AnimationExpenseCoins.SetEditAnimPosFinalY(_editAnimPosFinalY);
                    if (!isAd)
                    {
                        _AnimationExpenseCoins.StartAnimation(-_speenCost,false,true);
                    }
                  
                    Invoke("SectionCodeSpeen", _AnimationExpenseCoins.GetTimeToMove() + 0.6f);
                }
                else
                {
                    GameManager.Instance.EnableEventSystem();
                    AnalyticsManager.Instance.LogEvent("wheel_ad");
                    GameManager.Instance.HideFortune();
                    _RewardPanelAnimation.HideRewardPanel();
                    Invoke("ShowShop", 0.4f);
                }
            }
        }
       
    }

    private void ShowShop()
    {
        //_shopGameObj.GetComponent<AnimationPanel>().Show();
        GameManager.Instance.OpenShop();
    }

    private void SectionCodeSpeen()
    {
        _coinsButton.enabled = false;
        _rewardButton.enabled = false;

        _coinsButtonCanvas.DOFade(0, 0.5f);
        _rewardButtonCanvas.DOFade(0, 0.5f);

        _tween.Pause().SetDelay(0.1f);

        if (_state == false)
        {
            _random = (360 * 2 + (45 * Random.Range(1, 9)));


            _tween = _wheel.DORotate(new Vector3(0, 0, -_random), _speed * 0.8f).SetEase(_curve);
            wheelSpeen?.Invoke(_speed * 0.8f);

            _tween.onComplete += () =>
            {
                int i = (int)_wheel.transform.rotation.eulerAngles.z;

                wheelEnd?.Invoke();

                EnableAnimObjs();
                //Reward(i);
                Invoke("RewardInvoke", 1.5f);

                _state = true;
            };
        }

        else
        {
            _random = (360 * Random.Range(3, 5) + (45 * Random.Range(1, 9)));

            _tween = _wheel.DORotate(new Vector3(0, 0, -_random), _speed * 1.25f).SetEase(_curve);
            wheelSpeen?.Invoke(_speed * 1.25f);

            _tween.onComplete += () =>
            {
                int i = (int)_wheel.transform.rotation.eulerAngles.z;

                wheelEnd?.Invoke();

                Debug.LogError("--stopwheel1--");

                EnableAnimObjs();
                //Reward(i);
                Invoke("RewardInvoke", 1.5f);
            };
        }

        
    }

    private void RewardInvoke()
    {
        Reward((int)_wheel.transform.rotation.eulerAngles.z);

        
        Invoke("DisableAnimObjs", 1);
    }

    private void DisableAnimObjs()
    {
        GameManager.Instance.EnableEventSystem();
        _animEffectWheelObj1.SetActive(false);
        _animEffectWheelObj2.SetActive(false);
        _animEffectWheelObj3.SetActive(false);
    }
    private void EnableAnimObjs()
    {
        _animEffectWheelObj1.SetActive(true);
        _animEffectWheelObj2.SetActive(true);
        _animEffectWheelObj3.SetActive(true);
    }

    private void Reward(int index)
    {
        Debug.Log("Reward angler" + index);

        if (340 < index)
        {
            _indexReward = 0;

            StartCoroutine(ShowRewardCoroutine(0));
        }

        if (index < 20)
        {
            _indexReward = 0;

            StartCoroutine(ShowRewardCoroutine(0));
        }

        if (25 < index && index < 65)
        {
            _indexReward = 1;

            StartCoroutine(ShowRewardCoroutine(1));
        }

        if (70 < index && index < 110)
        {
            _indexReward = 2;

            StartCoroutine(ShowRewardCoroutine(2));
        }

        if (115 < index && index < 155)
        {
            _indexReward = 3;

            StartCoroutine(ShowRewardCoroutine(3));
        }

        if (160 < index && index < 200)
        {
            _indexReward = 4;

            StartCoroutine(ShowRewardCoroutine(4));
        }

        if (205 < index && index < 245)
        {
            _indexReward = 5;

            StartCoroutine(ShowRewardCoroutine(5));
        }

        if (250 < index && index < 290)
        {
            _indexReward = 6;

            StartCoroutine(ShowRewardCoroutine(6));
        }

        if (295 < index && index < 335)
        {
            _indexReward = 7;

            StartCoroutine(ShowRewardCoroutine(7));
        }
    }
    #endregion

    private IEnumerator ShowButton()
    {
        _coinsButtonCanvas.alpha = 0;
        _rewardButtonCanvas.alpha = 0;

        yield return new WaitForSeconds(1.5f);

        _coinsButtonCanvas.DOFade(1,0.5f).onComplete += () => 
        {
            _coinsButton.enabled = true;
            _rewardButton.enabled = true;
        };

        _rewardButtonCanvas.DOFade(1, 0.5f);

        _coinsButton.enabled = true;
        _rewardButton.enabled = true;
    }

    private IEnumerator ShowRewardCoroutine(int index)
    {
        yield return new WaitForSeconds(1f);
        ShowReward(index);
    }

    private void ShowReward(int index)
    {
        _rewardWindow.SetActive(true);
        _rewardCanvasGroup.DOFade(1, 0.5f);
        _shinningReward.DOFade(1, 0.1f).SetDelay(0.1f);
        _shinningRewardSecond.DOFade(1, 0.1f).SetDelay(0.1f);
        _rewardImage[index].SetActive(true);
    }

    public void EnableInteractableOkBtn()
    {
        _okButtonTakeGift.interactable = true;
    }

    public void TakeAGift()
    {
        float delay = 0.75f;

        Invoke("EnableInteractableOkBtn", 3f);
        
        switch (_indexReward)
        {
            case 0:
                HideRewardPanel();
                GameManager.Instance.AddFreeSpin();
                //free skin fortune
                Debug.Log(_prizeText[_indexReward]);
                break;
            case 1:
                Invoke(nameof(HideRewardPanel), delay);

                _animationCollectCoins.SetStartPosition(_rewardImage[1].GetComponent<RectTransform>());
                _animationCollectCoins.StartAnimation(20);

                // монеты 20
                Debug.Log(_prizeText[_indexReward]);
                break;
            case 2:
                HideRewardPanel();
                GameManager.Instance.AddKeyFree();
                //keyfree 1x
                Debug.Log(_prizeText[_indexReward]);
                break;
            case 3:
                HideRewardPanel();
                GameManager.Instance.AddTimer(2, false);
                //timer x2
                Debug.Log(_prizeText[_indexReward]);
                break;
            case 4:
                Invoke(nameof(HideRewardPanel), delay);

                _animationCollectCoins.SetStartPosition(_rewardImage[1].GetComponent<RectTransform>());
                _animationCollectCoins.StartAnimation(1000);
                // монеты 1000
                Debug.Log(_prizeText[_indexReward]);
                break;
            case 5:
                HideRewardPanel();
                GameManager.Instance.AddReviveFree();
                //revive 1x
                Debug.Log(_prizeText[_indexReward]);
                break;
            case 6:
                HideRewardPanel();
                GameManager.Instance.AddSaw(3,false);
                //saw x3
                Debug.Log(_prizeText[_indexReward]);
                
                break;
            case 7:
                Invoke(nameof(HideRewardPanel), delay);

                _animationCollectCoins.SetStartPosition(_rewardImage[1].GetComponent<RectTransform>());
                _animationCollectCoins.StartAnimation(100);
                // монеты 100
                Debug.Log(_prizeText[_indexReward]);
                break;
        }

        
    }

    private void HideRewardPanel()
    {
        _shinningReward.DOFade(0, 0.1f);
        _shinningRewardSecond.DOFade(0, 0.1f);

        _rewardCanvasGroup.DOFade(0, 0.5f).onComplete += () =>
        {
            _rewardWindow.SetActive(false);
        };

        _rewardImage[_indexReward].SetActive(false);

        _choiseFrame.DOFade(0, 0.5f);

        _coinsButton.enabled = true;
        _rewardButton.enabled = true;

        _coinsButtonCanvas.DOFade(1, 0.5f);
        _rewardButtonCanvas.DOFade(1, 0.5f);

        _rewardPanelAnimation.UPFill();

        GoalsEventManager.SendUpSpinTheWheelOfFortune();
    }
}
