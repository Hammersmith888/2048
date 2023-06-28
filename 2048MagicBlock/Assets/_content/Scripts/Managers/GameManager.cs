using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using EasyButtons;


public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [Header("Sound")]
    [SerializeField] public AudioPlayFunc _btnClickSound;
    [SerializeField] public AudioPlayFunc _popUpSound;
    [SerializeField] public AudioPlayFunc _coinColectSound;
    [SerializeField] public AudioPlayFunc _coinsColectSound;
    [SerializeField] public AudioPlayFunc _clockSound;
    [SerializeField] public AudioPlayFunc _specialBlockSound;
    [SerializeField] public AudioPlayRandom _mergeSound;
    [SerializeField] public AudioPlayFunc _rewardSound;
    [SerializeField] public AudioPlayFunc _sawSound;
    [SerializeField] public AudioPlayFunc _selectRewardSound;
    [SerializeField] public AudioPlayFunc _skinUnlockedSound;
    [SerializeField] public AudioPlayFunc _specialOfferSound;

    [HideInInspector] public bool _canPlayMusicSetting = true;
    [HideInInspector] public bool _canPlaySoundSetting = true;

    [Header("Timer")]
    [SerializeField] public Slider _timerSlider;

    [Header("Animation Cells")]
    [SerializeField]
    public RectTransform moneyStatIconPosition;

    [Header("Booster tutorial")]
    [SerializeField] public Canvas timerIconCanvas;
    [SerializeField] public Canvas sawIconCanvas;
    [SerializeField] public Canvas anotherTimerObjsCanvas;
    [SerializeField] public Canvas anotherSawObjsCanvas;
    [SerializeField] public GameObject mainSawObj;
    [SerializeField] public GameObject mainTimerObj;

    [Header("Fortune Whell")]
    [SerializeField] public Text textStopWhellFree;
    [SerializeField] public Text textStopWhellCost;

    [Header("----------------")]
    [SerializeField] private CanvasGroup _timerWheelFortuneCanvas, _buttonWheelFortune, _blackBG;
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private int _countTimer;
    [SerializeField] private GameObject _rouletteAndUpBlock;

    [SerializeField] private AnimationPanel _shop, _specialOffer, _VIP, _ranked, _dealyGoals;
    [SerializeField] private PausePanel _pause;

    [SerializeField] private LightLogic _lightFortune;

    [SerializeField] private Material _materialForDeadAnim;
    private int _reviveFree;
    [HideInInspector] public int reviveFree
    {
        get { return _reviveFree; }
        set
        {
            _reviveFree = value;
            PlayerPrefs.SetInt("reviveFree", _reviveFree);
        }
    }

    private UnityEngine.EventSystems.EventSystem _eventSystem;

    [Button("Reset all data")]
    public void ResetAllData()
    {
        gameData = default;
        GameData.dailyRewardDay = 0;
        GameData.isTutorialShowedUp = false;
        GameData.isCouplesShowedUp = false;
        gameData.SaveData();
    }
    [Button("Show daily reward")]
    public void ShowDailyRewardTest()
    {
        GameData.dailyRewardDay = 0;
        dailyRewardPanel.CheckDaily();
    }
    [Serializable]
    public struct GameScreenInstance
    {
        public GameScreen screenType;
        public AnimationPanel instance;
    }

    public GameData gameData;

    private Timer fieldValidTimer;
    private Timer wheelFortuneActiveTimer;

    public TextMeshProUGUI maxValueCounterText;

    public Transform PriorityShowParent;

    public Fortune wheelFortune;

    [SerializeField]
    private DailyRewardPanel dailyRewardPanel;

    [Header("Stats")]
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI reviveText;
    [SerializeField] private Text reviveTextFree;
    [SerializeField] private Text reviveTextSimple;

    public AnimationPanel moneyFortune;

    public GameObject FadeBackground;
    [SerializeField]
    private AnimationPanel GameOverScreen;
    [SerializeField]
    public AnimationPanel RevivePopup;
    [SerializeField]
    private GameScreenInstance[] gameScreens;

    [SerializeField]
    private AnimationPanel FreeCoins;



    [SerializeField]
    public PopupShow packagePopup;

    [SerializeField]
    private GameObject[] hideUITutorial;

    private TwiceEscape twiceEscape = new TwiceEscape();

    public int Score => cellLogic.Score;

    public Slider loadingSlider;

    private BalanceLoader balanceLoader;

    [SerializeField]
    private Transform wheelButtonAnimation;

    [SerializeField]
    private Transform moneyCellButtonAnimation;

    [SerializeField]
    private Transform keyButtonAnimation;

    [SerializeField]
    private GameObject wheelFortuneButton;

    public GameObject packageButton;

    [SerializeField]
    private Button endlessButton;

    public TapToPlay tapToPlay;

    public GameObject fortuneTimerGO;
    public TextMeshProUGUI fortuneTimerText;

    [Header("ДЛЯ отслеживания состояния")]
    public bool isGameOverScreen;
    public bool isPause = false;

    private CellLogic cellLogic => CellLogic.Instance;
    public TutorialLogic tutorialLogic => TutorialLogic.Instance;
    private CouplesLogic couplesLogic => CouplesLogic.Instance;
    public BoosterLogic boosterLogic => BoosterLogic.Instance;
    private Image _bg;
    private Canvas _bgCanvas;
    private int _bgCanvasOrderDefault;

    private bool _rankedState;
    private bool _menuState;

    private bool _isStartedCourutineTimerWheel;

    public GameScreen CurrentScreen { get; private set; }

    public void UpdateEndlessButton()
    {
        endlessButton.interactable = gameData.endlessUnlocked;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            tapToPlay.Show();
        }
    }

    public void OpenFreeCoins(int val)
    {
        var btn = FreeCoins.GetComponentsInChildren<Button>()[1];
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            CoinPurse.Instance.IncreaseCoins(val);
            FreeCoins.Hide();
            HideBlackBG();
            _blackBG.GetComponent<CanvasGroup>().alpha = 0;
            _blackBG.GetComponent<Canvas>().sortingOrder = 7;
        });
        FreeCoins.Show();
        ShowBlackBG();
        _blackBG.GetComponent<CanvasGroup>().alpha = 1;
        _blackBG.GetComponent<Canvas>().sortingOrder = 10;
    }

    private void Awake()
    {
        reviveFree =  PlayerPrefs.GetInt("reviveFree",0);

        _eventSystem = UnityEngine.EventSystems.EventSystem.current;

        _bg = _blackBG.GetComponent<Image>();
        _bgCanvas = _blackBG.GetComponent<Canvas>();
        _bgCanvasOrderDefault = _bgCanvas.sortingOrder;

        _timerWheelFortuneCanvas.alpha = 0;

        ChangeScreen(GameScreen.Loading);
        loadingTargetValue = 1.01f;

        gameData = new GameData() { score = 0 };      //В будующем можно переделать в загрузку 

        cellLogic.InitCells();

        gameData.LoadData();
        gameData.SaveData();

        UpdateEndlessButton();

        FortuneWheelController.Instance.wheelSpeen += (float val) => { wheelTimerRoutine = StartCoroutine(WheelTimer(val)); };
        FortuneWheelController.Instance.wheelEnd += () => { StopCoroutine(wheelTimerRoutine); fortuneTimerGO.SetActive(false); _rewardSound.PlaySound(); };

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        if (PlayerPrefs.GetInt("_canPlayMusicSetting", 1) == 1)
            _canPlayMusicSetting = true;
        else
            _canPlayMusicSetting = false;

        if (PlayerPrefs.GetInt("_canPlaySoundSetting", 1) == 1)
            _canPlaySoundSetting = true;
        else
            _canPlaySoundSetting = false;
    }

    public void DisableEventSystemLateEnable(float delayEnable)
    {
        _eventSystem.enabled = false;
        Invoke("EnableEventSystem", delayEnable);
    }

    public void DisableEventSystem()
    {
        _eventSystem.enabled = false;
    }

    public void EnableEventSystem()
    {
        _eventSystem.enabled = true;
    }
    public void EnableEventSystem(float timeInvoke)
    {
        Invoke("EnableEventSystem", timeInvoke);
    }

    public void EnableDisableMusic()
    {
        if (_canPlayMusicSetting)
        {
            _canPlayMusicSetting = false;
            PlayerPrefs.SetInt("_canPlayMusicSetting", 0);
            BackgroundMusicInstance.Instance.PauseMusic();
        }
        else
        {
            _canPlayMusicSetting = true;
            PlayerPrefs.SetInt("_canPlayMusicSetting", 1);
            BackgroundMusicInstance.Instance.ContinueMusic();
        }
    }
    public void EnableDisableSound()
    {
        if (_canPlaySoundSetting)
        {
            _canPlaySoundSetting = false;
            PlayerPrefs.SetInt("_canPlaySoundSetting", 0);
        }
        else
        {
            _canPlaySoundSetting = true;
            PlayerPrefs.SetInt("_canPlaySoundSetting", 1);
        }
    }

    public bool IsGameOver()
    {
        return cellLogic.IsGameOver();
    }

    private Coroutine wheelTimerRoutine;

    IEnumerator WheelTimer(float value)
    {
        float initVal = 0;

        fortuneTimerGO.SetActive(true);

        while (initVal < value)
        {
            initVal += Time.deltaTime;

            fortuneTimerText.text = (value - initVal).ToString();

            yield return null;
        }

        fortuneTimerGO.SetActive(false);
    }

    public void ShowRouletteAndUpBlock(int index)
    {
        _rouletteAndUpBlock.SetActive(true);
        _rouletteAndUpBlock.GetComponent<OpenAnimation>().OpenFortyne(index);
        //Debug.LogError("ShowRouletteAndUpBlock(int index)");
        //Show timer

    }

    private void StartTimerWheelFortune()
    {
        _timer.gameObject.SetActive(true);
        _timer.text = _countTimer.ToString();

        _timerWheelFortuneCanvas.DOFade(1, 0.5f).onComplete += () =>
        {
            if (!_isStartedCourutineTimerWheel)
            {
                _isStartedCourutineTimerWheel = true;
                StartCoroutine(TimerWheelFortune());
            }
        };
    }

    private IEnumerator TimerWheelFortune()
    {
        yield return new WaitForSeconds(1);
        if(!isPause)
            _countTimer--;
        _timer.text = _countTimer.ToString();
        if (_countTimer > 0)
        {
            StartCoroutine(TimerWheelFortune());
        }
        else
        {

            _timerWheelFortuneCanvas.DOFade(0, 0.5f);

            _buttonWheelFortune.DOFade(0, 0.5f).onComplete += () =>
            {
                //_timer.gameObject.SetActive(false);
                wheelFortuneButton.SetActive(false);

                if (GameManager.Instance.gameData.freeSpeenCount > 0)
                    GameManager.Instance.gameData.freeSpeenCount--;

                GameManager.Instance.textStopWhellFree.gameObject.SetActive(false);
                GameManager.Instance.textStopWhellCost.gameObject.SetActive(true);
                _isStartedCourutineTimerWheel = false;
            };
        }
    }

    private void InitTimers()
    {
        cellLogic.timer.delay = cellLogic.balance.timerSec;
        cellLogic.timer.OnReset += cellLogic.MoveGrid;

        fieldValidTimer.OnReset += cellLogic.CheckFieldValid;
        fieldValidTimer.delay = 0.25f;

        wheelFortuneActiveTimer.delay = cellLogic.balance.wheelButtonSec;
        wheelFortuneActiveTimer.OnReset += () => wheelFortuneButton.SetActive(false);
    }

    private float loadingValue;

    private float loadingTargetValue;

    private bool isLoaded;

    private void UpdateLoadingValue()
    {
        if (loadingValue < loadingTargetValue)
        {
            loadingValue += Time.deltaTime;
            loadingSlider.value = loadingValue;
        }
    }

    private IEnumerator GoogleSpeadsheetCourutine()
    {
        //GoogleSpeadsheetGet.instance.onLoadingUpdate += (float val) => loadingTargetValue = Mathf.Max(val, loadingTargetValue);

        GoogleSpeadsheetGet.instance.onGetData += (GoogleSpeadsheetGet.QueryType query, List<string> objTypeNames, List<string> jsonData) =>
        {
            //loadingTargetValue = 1.05f;
            // isLoaded = true;
            Debug.LogError("----------GoogleSpeadsheet Loaded");
        };
        yield return null;
    }

    public void AddReviveFree()
    {
        reviveFree++;
    }

    private void Start()
    {
        CheckToEnableBoosterIconInGame();

        StartCoroutine(GoogleSpeadsheetCourutine());
        
        if (PlayerPrefs.GetInt("RateUsShowed",0) == 0)
        {
            RateUS.Instance.ShowRateUSWithTimer(CellLogic.Instance.balance.timerRateUsShow1);
            RateUS.Instance.ShowRateUSWithTimer(
                CellLogic.Instance.balance.timerRateUsShow1 + CellLogic.Instance.balance.timerRateUsShow2
                );
        }
        else if (PlayerPrefs.GetInt("RateUsShowed", 0) == 1)
        {
            RateUS.Instance.ShowRateUSWithTimer(CellLogic.Instance.balance.timerRateUsShow2);
        }
    }

    public void CheckToEnableBoosterIconInGame()
    {
        if (PlayerPrefs.GetInt("BoosterSawTutorialShowed", 0) == 1)
        {
            boosterLogic.sawBooster.Enabled = true;
            mainSawObj.SetActive(true);
        }
        else
        {
            boosterLogic.sawBooster.Enabled = false;
            mainSawObj.SetActive(false);
        }

        if (PlayerPrefs.GetInt("BoosterTimerTutorialShowed", 0) == 1)
        {
            boosterLogic.timeBooster.Enabled = true;
            mainTimerObj.SetActive(true);
        }
        else
        {
            boosterLogic.timeBooster.Enabled = false;
            mainTimerObj.SetActive(false);
        }
    }

    private void StartScreen()
    {
        if (GameData.isTutorialShowedUp)
        {
            ChangeScreen(GameScreen.Menu);
            tutorialLogic.FinalizeTutorial();
            ChekSave();
        }
        else
        {
            ChangeScreen(GameScreen.Game);
            StartGame(false);
        }
    }

    public void StartGame(bool endless = false)
    {
        if (endless)
        {
            AnalyticsManager.Instance.LogEvent("endless_tap");
        }
        else
        {
            AnalyticsManager.Instance.LogEvent("play_tap");
        }
        ChekSave();

        gameData.isEndless = endless;

        cellLogic.cellsMaterial.SetFloat("_EffectAmount", 0);

        cellLogic.UpdateCellStep();
        UpdateStatsUI();

        ChangeScreen(GameScreen.Game);


        if (cellLogic.cells.Count == 0)
        {
            cellLogic.ShowStartDataSelect();
        }
    }

    public void ChekSave()
    {
        string MAP_KEY = "MapInfo";

        if (PlayerPrefs.HasKey(MAP_KEY))
        {
            foreach (Cell cell in CellLogic.Instance.cells)
                CellLogic.Instance.RemoveCell(cell);

            CellLogic.Instance.LoadSave();
            return;
        }
        UpdateStatsUI(0);
    }


    public enum GameScreen
    {
        Menu,
        Game,
        Loading
    }

    public void ChangeScreen(GameScreen screen)
    {
        gameScreens.First(n => n.screenType == screen).instance.ShowImmediate();
        if (AdsController.Instance != null)
        {
            AdsController.Instance.ResetTimeForAutoAds();
        }

        for (int i = 0; i < gameScreens.Length; i++)
        {
            if (gameScreens[i].screenType == screen)
            {
                gameScreens[i].instance.transform.SetAsFirstSibling();

                var canvas = gameScreens[i].instance.GetComponent<Canvas>();
                if (canvas != null)
                    Destroy(canvas);

                continue;
            }

            if (gameScreens[i].instance.gameObject.activeSelf)
            {
                gameScreens[i].instance.Hide();
                if (gameScreens[i].screenType == GameScreen.Game)
                {
                    var go = gameScreens[i].instance.gameObject;
                    var canvas = go.GetComponent<Canvas>();
                    if (canvas == null) canvas = go.AddComponent<Canvas>();

                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 5;
                }
            }
        }


        isPause = true;
        if (screen == GameScreen.Game)
        {
            cellLogic.timer.Reset(false);
            tapToPlay.Show();
        }

        CurrentScreen = screen;
    }

    private bool oneStage = false;
    private bool screenIsStarted = false;
    private bool HasTimeBoosterEnabled = false;

    private void CheckDailyRewards()
    {
        if (GameData.isTutorialShowedUp)
            dailyRewardPanel.CheckDaily();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.F))
        {
            AddReviveFree();
        }*/

        if (loadingValue < loadingTargetValue)
        {
            UpdateLoadingValue();
            if (loadingValue < loadingTargetValue && !isLoaded)
            {
                loadingTargetValue = 1.05f;
                isLoaded = true;
            }
        }
        else if (isLoaded && !screenIsStarted)
        {
            screenIsStarted = true;
            StartScreen();
            Invoke("CheckDailyRewards", 1);
        }
        if (loadingValue > 0.13f && !oneStage)
        {
            oneStage = true;

            if (GameData.isTutorialShowedUp)
            {
                //cellLogic.ShowStartDataSelect();
            }
            else
            {
                tutorialLogic.ShowTutorial();
            }

            cellLogic.balance.couplesMinSize = 2;
            cellLogic.balance.couplesAEnable = 10;
            cellLogic.balance.couplesMaxSizeA = 2;
            cellLogic.balance.couplesBEnable = 15;

            balanceLoader = new BalanceLoader();

            balanceLoader.onDataLoaded += (BalanceValues _balance) =>
            {
                cellLogic.balance = _balance;
                //if (GameData.isTutorialShowedUp)
                //  dailyRewardPanel.CheckDaily();
                InitTimers();
            };

            packagePopup.GetComponent<AnimationPanel>().onHideFinalize += () => { if (CurrentScreen == GameScreen.Game) isPause = false; };

            balanceLoader.Init();

            GoogleSpeadsheetGet.instance.RetrieveParameters();
            GoogleSpeadsheetGet.instance.UnpackJsonLocalData();
        }

        cellLogic.AnimateScore();
        cellLogic.CheckCellHighlight();
        couplesLogic.MoveCouplesDown();
        twiceEscape.Update();
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        
        if (wheelFortuneButton.gameObject.activeSelf)
        {
            wheelFortuneActiveTimer.Update(Time.deltaTime);
        }


        if (!isPause && !cellLogic.IsMovingGrid && CurrentScreen == GameScreen.Game && PopupController.Instance.activePopup == null)
        {
            fieldValidTimer.Update(Time.deltaTime);
        }


        if (CurrentScreen == GameScreen.Game && !isPause && GameData.isTutorialShowedUp && PopupController.Instance.activePopup == null && !GameManager.Instance.isGameOverScreen)
        {
            cellLogic.timer.Update(Time.deltaTime);
            cellLogic.timerSlider.value = 1f - (cellLogic.timer.value / cellLogic.timer.delay);

        }

        if (
            PlayerPrefs.GetInt("HasTimeBoosterEnabled", 0) == 0 
            && cellLogic.timer.value > cellLogic.timer.delay - cellLogic.balance.timeBoosterHelp)
        {
            HasTimeBoosterEnabled = true;
            if (PlayerPrefs.GetInt("HasTimeBoosterEnabled", 0) == 0)
            {
                AddTimer(1,true); 
            }
            PlayerPrefs.SetInt("HasTimeBoosterEnabled", 1);
            
        }
    }
    public void InvokeTimeBoosterEnableOnPanel(float timeSec)
    {
        Invoke("TimeBoosterEnableOnPanel", timeSec);
    }
    private void TimeBoosterEnableOnPanel()
    {
        if (!boosterLogic.timeBooster.Enabled)
        {
            boosterLogic.timeBooster.Enabled = true;
            anotherTimerObjsCanvas.sortingOrder = 8;
            timerIconCanvas.sortingOrder = 8;
            mainTimerObj.SetActive(true);
        }
        else if (!boosterLogic.sawBooster.Enabled)
        {
            boosterLogic.sawBooster.Enabled = true;
            anotherSawObjsCanvas.sortingOrder = 8;
            sawIconCanvas.sortingOrder = 8;
            mainSawObj.SetActive(true);
        }

    }

    public void OpenRanked()
    {
        _ranked.Show();
        _pause.Hide();
        _rankedState = false;
    }
    public void OpenRankedGameOver()
    {
        _ranked.Show();
        GameOverScreen.Hide();
        _rankedState = true;
    }

    public void OpenRankeMainMenu()
    {
        AnalyticsManager.Instance.LogEvent("ranking_open");
        _menuState = true;
        _rankedState = true;
        _ranked.Show();
        ShowBlackBG();
    }

    public void CloseRanked()
    {
        _ranked.Hide();

        if (_menuState)
        {
            HideBlackBG();
            _menuState = false;
        }

        else if (_rankedState == false && _menuState == false)
            _pause.Show();
        else
        {
            GameOverScreen.Show();

        }
    }

    public void OpenSpecialOffer()
    {
        AnalyticsManager.Instance.LogEvent("offer_tap");
        _specialOffer.Show();
        ShowBlackBG();
    }

    public void CloseSpecialOffer()
    {
        _specialOffer.Hide();
        HideBlackBG();
    }

    public void OpenShop()
    {
        AnalyticsManager.Instance.LogEvent("shop_tap");
        _shop.Show();
        ShowBlackBG();
    }
    public void OpenShop(float delay)
    {
        Invoke("OpenShop",delay);
    }

    public void CloseShop()
    {
        _shop.Hide();
        if (!GameManager.Instance.RevivePopup.IsActive)
        {
            HideBlackBG();
        }
        if (CurrentScreen == GameScreen.Game)
            StartCoroutine(PauseOff());
    }

    private IEnumerator PauseOff()
    {
        yield return new WaitForSeconds(0.6f);
        isPause = false;
    }

    public void SetBlackBGOrder(int value)
    {
        _bgCanvas.sortingOrder = value;
    }

    public void ResetBlackBGOrder()
    {
        _bgCanvas.sortingOrder = _bgCanvasOrderDefault;
    }

    public void ShowBlackBG()
    {
        Debug.Log("Show Black bg");
        _blackBG.DOFade(1, 0.5f);
        _bg.raycastTarget = true;
        _bg.GetComponent<GraphicRaycaster>().enabled = true;
    }

    public void HideBlackBG()
    {
        Debug.Log("Hide black bg");
        _blackBG.DOFade(0, 0.5f);
        Invoke("BgDisableRaycast", 0.5f);
    }
    private void BgDisableRaycast()
    {
        _bg.raycastTarget = false;
        _bg.GetComponent<GraphicRaycaster>().enabled = false;
    }

    public void OpenFortune()
    {
        //wheelFortune.GetComponent<AnimationPanel>().Show();
        DisableEventSystem();
        PopupController.Instance.ShowPopup(wheelFortune.GetComponent<AnimationPanel>());
        isPause = true;
        //ShowBlackBG();
        _lightFortune.StartLight();
    }

    public void HideFortune()
    {
        DisableEventSystem();
        wheelFortune.GetComponent<AnimationPanel>().Hide();
        if (CurrentScreen == GameScreen.Game)
            isPause = false;
        HideBlackBG();
    }

    public void OpenVIP()
    {
        AnalyticsManager.Instance.LogEvent("vip_open");
        PopupController.Instance.ShowPopup(_VIP/*, true*/);
        //PopupController.Instance.ShowPopupVIP(_VIP);
    }

    public void HideVIP()
    {
        PopupController.Instance.HidePopup(_VIP, true);
    }

    #region STATS

    public void AddTimer()
    {
        boosterLogic.timeBooster.Increase(1);
    }

    public void AddSaw()
    {
        boosterLogic.sawBooster.Increase(1);
    }
    public void AddFreeSpin()
    {
        gameData.freeSpeenCount++;

        if (gameData.freeSpeenCount > 0)
        {
            GameManager.Instance.textStopWhellFree.gameObject.SetActive(true);
            GameManager.Instance.textStopWhellCost.gameObject.SetActive(false);
        }
    }

    public void Del1FreeSpin()
    {
        gameData.freeSpeenCount--;

        if (gameData.freeSpeenCount < 1)
        {
            GameManager.Instance.textStopWhellFree.gameObject.SetActive(false);
            GameManager.Instance.textStopWhellCost.gameObject.SetActive(true);
        }
    }
    public void AddKeyFree()
    {
        BlocksSkinControllerNew.Instance.Add1KeyToOpenSkin();
    }

    public void AddTimer(int val, bool isShowPopup = true)
    {
        boosterLogic.timeBooster.Increase(val, isShowPopup);
    }

    public void AddSaw(int val, bool isShowPopup = true)
    {
        boosterLogic.sawBooster.Increase(val, isShowPopup);
    }

    public void AddMoney(int value)
    {
        //gameData.money += value;
        CoinPurse.Instance.IncreaseCoins(value);

        UpdateStatsUI();
    }
    public void AddMoneyWithDelay(int value, float delay)
    {
        StartCoroutine(AddMoneyWithDelayCourutine(value, delay));
    }
    public IEnumerator AddMoneyWithDelayCourutine(int value, float delay)
    {
        yield return new WaitForSeconds(delay);
        AddMoney(value);
    }

    public void UpdateStatsUI(int startValueScore = 9999999)
    {
        if (startValueScore != 9999999)
            gameData.score = startValueScore;

        scoreText.text = gameData.score.ToString();

        if (PlayerPrefs.GetInt("highScore1", 0) < GameData.highScore)
            PlayerPrefs.SetInt("highScore1", GameData.highScore);

        highScoreText.text = PlayerPrefs.GetInt("highScore1", 0).ToString();

        Saver.Instance.SaveData();
        //Debug.LogError(gameData.score);
    }

    #endregion
    #region TUTORIAL

    #endregion

    public void AnimateKeyButton(Vector3 begin, Vector3 end)
    {
        keyButtonAnimation.position = begin;
        keyButtonAnimation.gameObject.SetActive(true);
        var tween = keyButtonAnimation.DOMove(end, 0.75f);

        keyButtonAnimation.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        keyButtonAnimation.DOScale(1.5f, 0.25f).onComplete += () =>
        {
            keyButtonAnimation.DOScale(0.75f, 0.5f);
        };

        isPause = true;

        tween.onKill += () =>
        {
            OnKeyAdded();
            keyButtonAnimation.gameObject.SetActive(false);
            packageButton.SetActive(true);
            //isPause = false;

            packagePopup.gameObject.SetActive(true);
            packagePopup.ShowPopup();
            packagePopup.GetComponent<PackagePanel>().SetFree();
            //isPause = true;
        };

    }

    public void OnKeyAdded()
    {
        BlocksSkinController.Instance.UnlockNewSkin();
    }

    public void AnimateWheelFortuneButton(Vector3 begin, Vector3 end)
    {
        wheelButtonAnimation.position = begin;
        wheelButtonAnimation.gameObject.SetActive(true);
        var tween = wheelButtonAnimation.DOMove(end, 0.75f);

        wheelButtonAnimation.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        wheelButtonAnimation.DOScale(1.5f, 0.25f).onComplete += () =>
        {
            wheelButtonAnimation.DOScale(0.75f, 0.5f);
        };

        isPause = true;
        Debug.Log("In animation");
        tween.onKill += () =>
        {
            wheelButtonAnimation.gameObject.SetActive(false);
            _buttonWheelFortune.alpha = 1;
            _countTimer = 30;
            wheelFortuneButton.SetActive(true);
            Debug.Log("Start timer");
            StartTimerWheelFortune();
            isPause = false;
        };


    }

    public void AnimateMoneyCellButton(Vector3 begin, Vector3 end, int moneyChange = 0)
    {
        moneyCellButtonAnimation.position = begin;
        moneyCellButtonAnimation.gameObject.SetActive(true);
        var tween = moneyCellButtonAnimation.DOMove(end, 1.3f);


        moneyCellButtonAnimation.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        moneyCellButtonAnimation.DOScale(1.5f, 0.25f).onComplete += () =>
        {
            moneyCellButtonAnimation.DOScale(1.0f, 0.5f);

            if (moneyChange != 0)
            {
                _coinColectSound.PlaySound(1f);
                AddMoneyWithDelay(moneyChange, 1.3f);
            }
        };

        //isPause = true;
        Debug.Log("In animation");
        tween.onKill += () =>
        {
            moneyCellButtonAnimation.gameObject.SetActive(false);
            //_buttonWheelFortune.alpha = 1;
            //_countTimer = 30;
            //wheelFortuneButton.SetActive(true);
            //Debug.Log("Start timer");
            //StartTimerWheelFortune();
            //isPause = false;
        };


    }

    #region MAIN

    public void MoveGridUp()
    {
        if (cellLogic.cells.Any(n => n.cellPosition.GridPos.y == 0))
            return;

        cellLogic.ResetTimer(true);
    }

    public void SendEmail()
    {
        Utilites.sendEmail("example@example.com", "Test", "This is a text\r\nAnother test\r\nAnd another text");
    }

    public void SetActiveUI(bool val)
    {
        for (int i = 0; i < hideUITutorial.Length; i++)
        {
            hideUITutorial[i].SetActive(val);
        }

        //cellLogic._25counterObject.SetActive(gameData.isEndless);
    }


    public void GameOver()
    {
        isGameOverScreen = true;
        //gameData.score = 0;
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        
        float val = 0;
        while (val < 1)
        {
            val += Time.deltaTime * 0.75f;
            //cellLogic.cells[0].image.material.SetFloat("_EffectAmount", val);
            _materialForDeadAnim.SetFloat("_EffectAmount", val);

            yield return null;
        }

        RevivePopup.Show();
        isPause = true;
        ShowBlackBG();
        reviveText.text = $"{revivePrice}";
        reviveTextSimple.text = $"{revivePrice}";

        if (reviveFree > 0)
        {
            reviveTextSimple.enabled = false;
            reviveTextFree.enabled = true;
        }
        else
        {
            reviveTextSimple.enabled = true;
            reviveTextFree.enabled = false;
        }

        _timerSlider.value = 0;
        //reviveText.text = $"Revive {revivePrice}";
    }

    public void GameOverClose()
    {
        isGameOverScreen = false;
        GameOverScreen.Hide();

        cellLogic.ClearCellsField();
        gameData.score = 0;
        cellLogic.scoreTarget = 1;
        Saver.Instance.SaveData();
        UpdateStatsUI();

        ChangeScreen(GameScreen.Menu);
    }

    public void ReviveAD()
    {
        AdsController.Instance.ShowRewardedInterstitialAd(() => {
            CoinPurse.Instance.IncreaseCoins(30);
            // gameData.money += 30;
            UpdateStatsUI();
            AnalyticsManager.Instance.LogEvent("revive_adwatch");

            if (!isFirstWatching)
            {
                return;
            }
            AnalyticsManager.Instance.LogEvent("revive_ad");
            HideBlackBG();
            isFirstWatching = false;
            ReviveEnd();
        });
        
    }

    private static bool isFirstWatching = true;
    private static int revivePrice = 30;

    public void TogglePause()
    {
        isPause = !isPause;
    }

    public void SetPause(bool value)
    {
        isPause = value;
    }

    public void Revive()
    {
        if (reviveFree > 0)
        {
            AnalyticsManager.Instance.LogEvent("revive_free");
            isPause = false;
            HideBlackBG();
            UpdateStatsUI();
            ReviveEnd();
            reviveFree--;
        }
        else if (CoinPurse.Instance.CostSubtraction(revivePrice,false))
        {
            AnalyticsManager.Instance.LogEvent("revive_coin");
            isPause = false;
            HideBlackBG();
            UpdateStatsUI();
            revivePrice += 30;
            ReviveEnd();
        }
     
    }

    private void ReviveEnd()
    {
        isGameOverScreen = false;
        isPause = false;
        cellLogic.cells[0].image.material.SetFloat("_EffectAmount", 0);
        _materialForDeadAnim.SetFloat("_EffectAmount", 0);
        RevivePopup.Hide();
        cellLogic.RemoveRandomCellsAnimated();

        GoalsEventManager.SendUpUseRevive1Time();
    }

    public void RestartGame()
    {
        AnalyticsManager.Instance.LogEvent("retry_tap");
        cellLogic.ClearCellsField();
        //boosterLogic.sawBooster.isShowedPopup = false;
        cellLogic.maxValue = 5;
        cellLogic.ShowStartDataSelect();
        isPause = false;
        isGameOverScreen = false;
        cellLogic.timer.Reset(false);
        gameData.score = 0;
        cellLogic.scoreTarget = 0;
        Saver.Instance.SaveData();
        HideBlackBG();
        _materialForDeadAnim.SetFloat("_EffectAmount", 0);
        //cellLogic.cells[0].image.material.SetFloat("_EffectAmount", 0);
        //UpdateStatsUI(0);
    }

    public void SetIsEnterInGamePrefs(bool IsEnterInGame)
    {
        UpdateStatsUI();
        if (IsEnterInGame)
            PlayerPrefs.SetInt("IsEnterInGame", 1);
        else
            PlayerPrefs.SetInt("IsEnterInGame", 0);
    }

    public void SaveAndLoadSave()
    {
        Saver.Instance.SaveData();
        ChekSave();
    }

    public void SaveGameCellsPosition()
    {
        Saver.Instance.SaveData();
    }

    #endregion

    #region Debug
    public void UpCoins()
    {
        CoinPurse.Instance.IncreaseCoins(1000);
    }
    #endregion


}


