using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoosterLogic : MonoBehaviourSingleton<BoosterLogic>
{
    [SerializeField] private BoosterPopup boosterDialogPopup;
    [SerializeField] private Image boosterDialogImage;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Transform boosterHand;

    private CouplesLogic couplesLogic => CouplesLogic.Instance;
    private CellLogic cellLogic => CellLogic.Instance;

    public Booster sawBooster;
    public Booster timeBooster;

    private Booster boosterPopup;

    public Image timerAnimationFadeObject;
    public ParticleSystem timerIconParticleSystem;

    public PopupShow sawPopup;
    public PopupShow timerPopup;

    private void Awake()
    {
       
        InitBoosters();
    }

    #region BOOSTERS

    //Двигаем иконку с диалога на место кнопки бустера
    private void MoveBoosterIconToButton(Booster booster)
    {
        GameManager.Instance.FadeBackground.SetActive(true);
        GameManager.Instance.isPause = true;
        booster.SetFading(false);

        var tween = boosterDialogPopup.previewIconImage.transform.DOMove(booster.AttachButton.transform.position, 1f);
        boosterDialogPopup.previewIconImage.transform.DOScale(boosterDialogPopup.endAnimScale, 1f);

        booster.AttachButton.interactable = false;

        tween.onKill += () => {
            if(booster == sawBooster)
            {
                boosterHand.transform.position = boosterDialogPopup.previewIconImage.transform.position;
                boosterHand.gameObject.SetActive(true);
                booster.AttachButton.interactable = true;
            }
            else
            {
                StartCoroutine(Utilites.ExecuteAfterTime(1f, () =>
                {
                    boosterHand.transform.position = boosterDialogPopup.previewIconImage.transform.position;
                    boosterHand.gameObject.SetActive(true);
                    var handImg = boosterHand.GetComponent<Image>();
                    handImg.color = Color.clear;
                    var handTween = handImg.DOColor(Color.white, 0.5f);
                    handTween.onKill += () => { booster.AttachButton.interactable = true; };
                }));
            }

        };
    }
    //Возвращаем игру в нормальное состояние (убираем паузу, убираем фейды)
    private void ResetBoosterTutorial(Booster booster)
    {
        

        var fadeImg = GameManager.Instance.FadeBackground.GetComponent<Image>();

        var fadeTween = fadeImg.DOColor(Color.clear, 0.5f);
        fadeTween.onKill += () => {
            fadeImg.gameObject.SetActive(false);
            fadeImg.color = Color.white;

            if (!booster.isAnimating)
                GameManager.Instance.isPause = false;

            booster.SetFading(true);

            boosterHand.gameObject.SetActive(false);

            boosterDialogPopup.ResetPreviewImage();
        };

    }
    //Устанавливаем бустеру логику
    private void SetupBooster(Booster booster, Action action, Func<bool> canActivate)
    {
        booster.canActivate = canActivate;
        /*

                booster.onActivate += () =>
                {

                    action?.Invoke(); 
                    ResetBoosterTutorial(booster);
                };

                booster.onClosePopup += () => {
                    if (booster.canActivate == null || booster.canActivate.Invoke())
                        MoveBoosterIconToButton(booster);
                };
         */

        booster.Init();
    }

    //Проверка можно ли использовать пилу в данный момент
    private bool CanActivateSawBooster()
    {
        return couplesLogic.couples.Count > 0;
    }

    private bool boosterIsActivated = false;
    private bool _startAnimation;
    //Инициализация бустеров
    private void InitBoosters()
    {
        boosterHand.gameObject.SetActive(false);

        timeBooster.canActivate = () => true;
        timeBooster.Init();

        timeBooster.onActivate += () =>
        {
            if (_startAnimation)
            {
                StartCoroutine(TimerFillingAnimation());
                return;
            }
            else
            {
                _startAnimation = true;
            }
            if (boosterIsActivated) return;
            boosterIsActivated = true;
            GoalsEventManager.SendUseTheClockOrSaw1Time();

            Debug.Log("Go to next return");
            timeBooster.boostAnimation.HideHand();

            timerAnimationFadeObject.gameObject.SetActive(true);
            timerAnimationFadeObject.color = Color.clear;
            timerAnimationFadeObject.DOColor(Color.white, 0.2f);

            var boosterImg = timerAnimationFadeObject.transform.GetChild(0).GetComponent<RectTransform>();
            boosterImg.anchoredPosition = Vector3.zero;
            boosterImg.transform.localScale = Vector3.zero;
            boosterImg.DOScale(Vector3.one * 4f, 1f);

            GameData.isCellsLocked = true;

            StartCoroutine(Utilites.ExecuteAfterTime(1.5f, () =>
            {
                boosterImg.DOScale(Vector3.one, 0.5f);

                var boosterMoveTween = boosterImg.DOMove(timerIconParticleSystem.transform.position, 0.5f);

                StartCoroutine(TimerFillingAnimation());

                var fadeAnim = timerAnimationFadeObject.DOColor(Color.clear, 0.5f);
                fadeAnim.onKill += () =>
                {
                    timerAnimationFadeObject.gameObject.SetActive(false);

                    timerIconParticleSystem.gameObject.SetActive(true);
                    timerIconParticleSystem.Play();
                    StartCoroutine(Utilites.ExecuteAfterTime(1f, () =>
                    {
                        timerIconParticleSystem.gameObject.SetActive(false);
                    }));

                    GameManager.Instance.HideBlackBG();
                    GameData.isCellsLocked = false;
                    GameManager.Instance.isPause = false;

                    boosterIsActivated = false;
                };



            }));
        };

        sawBooster.canActivate = CanActivateSawBooster;
        sawBooster.Init();
        sawBooster.onActivate += () =>
        {
            Debug.Log("Show saw booster ");
            couplesLogic.RemoveAllCouplesAnimation();
            sawBooster.boostAnimation.HideHand();
            GoalsEventManager.SendUseTheClockOrSaw1Time();
            GameManager.Instance._sawSound.PlaySound();
        };

        
    }

    //Показать диалог для бустера
    public void ShowBoosterDialogPopup(Booster booster, bool justSaveData = false)
    {
        if (justSaveData)
        {
            boosterDialogPopup.UpdateData(booster);
        }
        else
        {
            if (!(booster.canActivate == null || booster.canActivate.Invoke()))
            {
                boosterPopup = null;
                return;
            }

            if (booster == sawBooster)
                boosterDialogPopup = sawPopup.GetComponent<BoosterPopup>();
            else
                boosterDialogPopup = timerPopup.GetComponent<BoosterPopup>();

            Debug.Log(GameManager.Instance.wheelFortune.AnimationPanel.IsActive);
            if (GameManager.Instance.wheelFortune.AnimationPanel.IsActive)
            {
                GameManager.Instance.HideFortune();
            }


            boosterDialogPopup.UpdateData(booster);
            boosterDialogPopup.AnimationPanel.ShowPopup();
            boosterPopup = booster;

            GameManager.Instance.isPause = true;
            _gameManager.ShowBlackBG();
        }
    }
    //Спрятать диалог
    public void HideBoosterDialogPopup()
    {
        if (boosterPopup == null)
            return;

        boosterDialogPopup.AnimationPanel.HidePopup();
        GameManager.Instance.isPause = false;
        _gameManager.HideBlackBG();

        boosterPopup.onClosePopup?.Invoke();
        boosterPopup = null;

    }

    #endregion

    private IEnumerator TimerFillingAnimation()
    {
        float val = 0;
        var startTimerValue = cellLogic.timer.value;
        Debug.Log("TimerFillAnimation");
        GameManager.Instance._clockSound.PlaySound();
        while (val< 1f)
        {
            val += Time.deltaTime * 2f;
            
            Debug.Log("Setvalue timer");
            cellLogic.timer.SetValue(Mathf.Lerp(startTimerValue, 0, val));
            cellLogic.timerSlider.value = 1f - (cellLogic.timer.value / cellLogic.timer.delay);
            GameManager.Instance.UpdateStatsUI();

            yield return null;
        }
        Debug.Log("Setvalue timer11");
        cellLogic.timer.SetValue(0f);

        GameManager.Instance._clockSound.GetComponent<AudioSource>().Stop();

        var sliderFill = cellLogic.timerSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        var fadeSliderTween  = sliderFill.DOColor(Color.gray, 0.1f);
        fadeSliderTween.onKill += () => { sliderFill.DOColor(Color.white, 0.1f); };
    }
}

