using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BoostAnimation : MonoBehaviour
{
    [SerializeField] private Button _continueButton;

    [SerializeField] private Transform _boostImage, _boostImagePanel, _game;

    [SerializeField] private GameObject _boostButton, _upCountButton, _group, _hand;  

    [SerializeField] private Button _okButton;
    [SerializeField] private float _delay;

    [SerializeField] private ParticleSystem _bounce;

    [SerializeField] private CanvasGroup _shinning;

    [SerializeField] AnimationCurve _curve;

    private CanvasGroup _boostButtonCanvas, _upCountCanvas, _boostImageCanvas, _popupCanvas, _handCanvas;

    private void Awake()
    {
      //  _upCountCanvas = _upCountButton.GetComponent<CanvasGroup>();
      //  _boostButtonCanvas = _boostButton.GetComponent<CanvasGroup>();
        _boostImageCanvas = _boostImage.GetComponent<CanvasGroup>();
        _popupCanvas = gameObject.GetComponent<CanvasGroup>();
        _handCanvas = _hand.GetComponent<CanvasGroup>();
    }

    public void EnableInteractableOkBtn()
    {
        _okButton.interactable = true;
    }

    public void HideHand()
    {
        _handCanvas.DOFade(0, 0.5f).onKill 
            += () => {
            _hand.SetActive(false);
        };
    }

    public void StartMoveBoost()
    {
        Invoke("EnableInteractableOkBtn",3f);

        GameManager.Instance.InvokeTimeBoosterEnableOnPanel(1.5f);
        /*previous*/

        _hand.SetActive(false);
        _handCanvas.alpha = 0;
        //_continueButton.interactable = false;
        _popupCanvas.DOFade(0, 1).SetDelay(1f);
        _popupCanvas.DOFade(0, 0.5f).SetDelay(0.5f).onComplete += () =>
        {
            gameObject.SetActive(false);
        };

        _boostImage.transform.SetParent(_game.transform);

        _boostImage.DOJump(_boostImagePanel.transform.position, 3, 1, 1f).SetDelay(0.5f).SetEase(_curve).onComplete += () => 
        {
            _group.SetActive(true);
            _boostButton.SetActive(true);
           // _upCountButton.SetActive(true);

           // _upCountCanvas.alpha = 0;
          //  _boostButtonCanvas.alpha = 1;

            _boostImageCanvas.DOFade(0,0.25f).SetDelay(0.5f);

            _boostImage.DOScale(0.8f, 0.25f).SetLoops(2,LoopType.Yoyo).onComplete += () => 
            {
                _bounce.Play();
            };

            _popupCanvas.DOFade(0, 0.5f).SetDelay(0.5f).onComplete += () => 
            {
                if (PlayerPrefs.GetInt("BoosterSawTutorialShowed", 0) == 0 
                || PlayerPrefs.GetInt("BoosterTimerTutorialShowed", 0) == 0)
                {
                    if (PlayerPrefs.GetInt("BoosterTimerTutorialShowed", 0) == 1)
                        PlayerPrefs.SetInt("BoosterSawTutorialShowed", 1);

                    if (PlayerPrefs.GetInt("BoosterTimerTutorialShowed", 0) == 0)
                        PlayerPrefs.SetInt("BoosterTimerTutorialShowed", 1);

                    var animationPanel = GetComponent<AnimationPanel>();

                    if (animationPanel != null)
                    {
                        animationPanel.HideImmediate();
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    //GameManager.Instance.HideBlackBG();
                    GameManager.Instance.ResetBlackBGOrder();
                    Debug.LogError("----Tutorial booster");
                    _hand.SetActive(true);
                    _handCanvas.DOFade(1f, 0.5f);

                }

            };

           // _upCountCanvas.DOFade(1,1).SetDelay(2);
        };

        _boostImage.DOScale(new Vector3(0.45f, 0.45f, 0.45f), 1f).SetDelay(0.5f);
    }
}
