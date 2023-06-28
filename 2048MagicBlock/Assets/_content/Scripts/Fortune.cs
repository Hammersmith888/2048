using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fortune : MonoBehaviour
{
    /*
    [SerializeField]
    private Transform rotatingImg;
    */
    [SerializeField]
    private Transform field;

    [SerializeField]
    private ArrowFortune arrow;

    [SerializeField]
    private TextMeshProUGUI moneyStat;

    [SerializeField]
    private Button stopButton;

    [SerializeField]
    private Button stopButton2;

    [SerializeField]
    private float speedFortune = 45f;

    

    public enum Rewards { 
        BOX = 0,
        _100 = 1,
        X3_SAW = 2,
        X2_ROCKET = 3,
        _1000 = 4,
        X2_TIMER = 5,
        X1_THEME = 6,
        _20 = 7
    }

    public AnimationPanel AnimationPanel
    {
        get
        {
            if (_animationPanel == null)
                _animationPanel = GetComponent<AnimationPanel>();
            return _animationPanel;
        }
    }
    private AnimationPanel _animationPanel;

    private void OnDisable()
    {
        StopAllCoroutines();
        isSpining = false;
    }

    private void Start()
    {
        stopButton.onClick.AddListener(ClickStop);
        stopButton2.onClick.AddListener(ClickStopAd);

        GetComponent<AnimationPanel>().onFadebackClick += () => { GameManager.Instance.HideFortune(); };
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        isSpining = false;

        stopButton.gameObject.SetActive(false);
        stopButton2.gameObject.SetActive(false);
        Init();
    }

    private void ClickStopAd()
    {
        StartCoroutine(StopWheel());
    }

    private void ClickStop()
    {
        //GameManager.instance.gameData.money -= 50;



        CoinPurse.Instance.CostSubtraction(50);


        GameManager.Instance.UpdateStatsUI();
        ClickStopAd();
    }

    public void Init() {
        //stopButton.interactable = GameManager.instance.gameData.money >= 50;
        //stopButton.interactable = CoinPurse.Instance.CoinsCount >= 50;

        stopButton.gameObject.SetActive(false);
        stopButton2.gameObject.SetActive(false);

        StartCoroutine(InitRoutine());
    }

    private void Update()
    {
        /*
        rotatingImg.Rotate(new Vector3(0, 0, Time.deltaTime * 10));
        */
        if (isSpining)
        {
            field.Rotate(new Vector3(0, 0, Time.deltaTime * speedFortune));
        }
    }

    private bool isSpining = false;

    private IEnumerator InitRoutine()
    {
        arrow.isActive = true;
        GameManager.Instance.isPause = true;

        float val = 0;

        while(val < 1f)
        {
            val += Time.deltaTime;

            field.Rotate(new Vector3(0, 0, Time.deltaTime * speedFortune*val));
 
            yield return null;
        }
        isSpining = true;

        yield return new WaitForSeconds(0.5f);

        stopButton.gameObject.SetActive(true);
        stopButton2.gameObject.SetActive(true);
        
        yield return null;
    }

    private IEnumerator StopWheel()
    {

        float val = 0;

        isSpining = false;

        stopButton.gameObject.SetActive(false);
        stopButton2.gameObject.SetActive(false);

        while (val < 1f)
        {
            val += Time.deltaTime;

            field.Rotate(new Vector3(0, 0, Time.deltaTime * speedFortune * (1f-val)));

            yield return null;
        }

        arrow.isActive = false;

        Rewards reward = (Rewards)arrow.lastCollider;

        ParseReward(reward);

        yield return new WaitForSeconds(1f);
        Debug.LogError("--stopwheel--");
        Init();
    }

    private void ParseReward(Rewards reward)
    {
        switch (reward)
        {
            case Rewards.BOX:
                //GameManager.Instance.gameData.freeSpeenCount++;

                //GameManager.Instance.textStopWhellFree.enabled = true;
                //GameManager.Instance.textStopWhellCost.enabled = false;

                Debug.Log("case Rewards.BOX");
                break;
            case Rewards._100:
                //GameManager.instance.gameData.money += 100;
                CoinPurse.Instance.IncreaseCoins(100);
                Debug.Log("case Rewards._100");
                break;
            case Rewards.X3_SAW:
                Debug.Log("case Rewards.X3_SAW");
                GameManager.Instance.AddSaw(3);
                break;
            case Rewards.X2_ROCKET:
                //GameManager.Instance.AddRocket();
                //GameManager.instance.AddRocket();
                //GameManager.instance.AddRocket();
                Debug.Log("case Rewards.X2_ROCKET");
                break;
            case Rewards._1000:
                //GameManager.instance.gameData.money += 1000;
                Debug.Log("case Rewards._1000");
                CoinPurse.Instance.IncreaseCoins(1000);
                break;
            case Rewards.X2_TIMER:
                GameManager.Instance.AddTimer(2);
                Debug.Log("case Rewards.X2_TIMER");
                break;
            case Rewards.X1_THEME:
                print("TODO!");
                Debug.Log("case Rewards.X1_THEME");
                break;
            case Rewards._20:
                //GameManager.instance.gameData.money += 20;
                CoinPurse.Instance.IncreaseCoins(20);
                Debug.Log("case Rewards._20");
                break;
        }

        GameManager.Instance.UpdateStatsUI();
        //moneyStat.text = GameManager.instance.gameData.money.ToString();
    }
}

