using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


 [System.Serializable]
public struct ButtonState
{
    public bool state;
    public bool isMusicBtn;
    public Color activeColor;
    public Color noActiveColor;

    public Image buttonImg;
    public Button button;

    public void Init()
    {
        if (isMusicBtn)
            state = GameManager.Instance._canPlayMusicSetting;
        else
            state = GameManager.Instance._canPlaySoundSetting;

        UpdateGraphics();
        button.onClick.AddListener(Toggle);
    }

    public void Toggle()
    {
        state = !state;
        UpdateGraphics();
        if (isMusicBtn)
            GameManager.Instance.EnableDisableMusic();
        else
            GameManager.Instance.EnableDisableSound();

        GameManager.Instance._btnClickSound.PlaySound();
    }

    public void UpdateGraphics()
    {
        buttonImg.color = state ? activeColor : noActiveColor;
    }
}


public class PausePanel : AnimationPanel
{
    [SerializeField]
    private ButtonState musicButton;
    [SerializeField]
    private ButtonState soundButton;

    [SerializeField]
    private Transform vipButton;

    [SerializeField] private GameManager _gameManager;

    private Vector3 initVipPos;

    public static Action PauseDelay;

    private void Start()
    {
        musicButton.Init();
        soundButton.Init();

        onShowPre += () => { vipButton.transform.DOLocalMove(initVipPos, 0.2f); };
    }

    public void Home()
    {
        Deactivate();
        Saver.Instance.SaveData();
        GameManager.Instance.ChangeScreen(GameManager.GameScreen.Menu);
      //  _gameManager.HideBlackBG();
    }

    public void Activate()
    {
        AnalyticsManager.Instance.LogEvent("pause_open");
        if(GameManager.Instance.isPause==true)
        {
            return;
        }
        Debug.Log("Activate");
        initVipPos = vipButton.transform.localPosition;
        vipButton.transform.localPosition = initVipPos - new Vector3(350, 0, 0);
        GameManager.Instance.isPause = true;
        Show();
        _gameManager.ShowBlackBG();
    }

    public void Deactivate()
    {
        AnalyticsManager.Instance.LogEvent("resume_tap");
        Debug.Log("Deactivate");
        vipButton.transform.localPosition = initVipPos;
        //GameManager.Instance.isPause = false;
        if (PauseDelay != null)
        {
            Debug.Log("Invoke");
            PauseDelay.Invoke();
        }
        Hide();
        _gameManager.HideBlackBG();
    }
}
