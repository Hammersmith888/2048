using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BoosterType
{
    Saw,
    Timer
}

[Serializable]
public class Booster 
{
    public string titleID;
    public string descriptionID;

    public BoostAnimation boostAnimation;

    public bool Enabled { 
        get => enabled; set 
        {
            enabled = value;
            Debug.Log("Enabled " + enabled);
            UpdateButtonActive(); 
        }
    }

    private bool enabled=false;

    public Action onActivate;
    public Func<bool> canActivate;
    public Action onShowPopup;
    public Action onClosePopup;

    public BoosterType type;

    public Sprite Icon;

    private Transform initParent;

    public Button AttachButton => attachButton;

    [SerializeField]
    private Button attachButton;
    [SerializeField]
    private Image alertImage;
    [SerializeField]
    private TextMeshProUGUI alertCounter;
    [SerializeField] private Image _moreBooster;
    [SerializeField] private Sprite _haveBoosterImage;
    [SerializeField] private Sprite _dontHaveBoosterImage;

    //Флаг для показа попапа в первый раз
    [HideInInspector]
    public bool isShowedPopup;

    public int Count { get; set; }

    public bool isAnimating;


    private void UpdateButtonActive()
    {
        attachButton.transform.parent.parent.gameObject.SetActive(enabled);
    }

    public void Init()
    {
       // attachButton.onClick.AddListener(Activate);

        UpdateButtonActive();
        UpdateGraphics();

        initParent = attachButton.transform.parent.parent;
    }

    public void SetFading(bool value)
    {
        if (value)
        {
            Debug.Log(initParent);
            attachButton.transform.SetParent(initParent);
        }
        else
        {
            attachButton.transform.SetParent(GameManager.Instance.PriorityShowParent);
        }
           
    }

    public void Increase(int value, bool isShowPopup = true)
    {
        Debug.LogError("--Increase  "+ value);
        attachButton.gameObject.SetActive(true);

        if (!isShowedPopup && isShowPopup)
        {
            isShowedPopup = true;

            BoosterLogic.Instance.ShowBoosterDialogPopup(this);
        }
        if (isShowPopup) BoosterLogic.Instance.ShowBoosterDialogPopup(this,true); ;

        Count += value;

        UpdateGraphics();
    }

    public void Activate()
    {
        if (canActivate != null && !canActivate.Invoke())
            return;

        if (Count <= 0)
            return;

        Count--;

        Enabled = true;

        onActivate?.Invoke();

        UpdateGraphics();
    }

    private void AddOneBooster()
    {
        Increase(1);
    }
    private void AddOneBooster(bool showTutorial)
    {
        Increase(1, showTutorial);
    }
    private void SetButtonAction()
    {
        attachButton.onClick.RemoveAllListeners();
        if (Count > 0)
        {
            
            attachButton.onClick.AddListener(Activate);
        }
        else
        {
            Debug.Log("add listener one booster");
            attachButton.onClick.AddListener(() => { AdsController.Instance.ShowRewardedInterstitialAd(() => { AddOneBooster(false); }); });
        }
    }

    public void UpdateGraphic()
    {
        UpdateGraphics();
    }

    protected virtual void UpdateGraphics()
    {
        // alertImage.gameObject.SetActive(Count > 0);
        SetButtonAction();
        _moreBooster.sprite = Count > 0 ? _haveBoosterImage : _dontHaveBoosterImage;
        alertCounter.text = $"{Count}";
    }
}