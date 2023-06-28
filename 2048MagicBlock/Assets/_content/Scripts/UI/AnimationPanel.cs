using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AnimationPanel : MonoBehaviour
{
    private const float animDuration = 0.7f;
    private const float firstDuration = animDuration * 0.7f;
    private const float secondDuration = animDuration * 0.3f;
    private const float movingDownOffset = 0.05f;

    public float AnimDuration => animDuration;
    public RectTransform MovingObject => movingObject;

    [HideInInspector,SerializeField]
    private Vector3 _initPosition;

    public event Action onShow;
    public event Action onShowPre;
    public event Action onFadebackClick;
    public event Action onHide;
    public event Action onHideFinalize;

    public bool ignoreItAsPopup = false;

    public bool IsActive { 
        get => gameObject.activeSelf;
        private set {
            gameObject.SetActive(value);
        }
    }

    private Button button;

    [SerializeField] protected RectTransform movingObject;
    [SerializeField] private bool _playButtonSound = true;
    [SerializeField] private bool _popUpSound = true;
    [SerializeField] private bool _specialOfferSound = false;
    [SerializeField] private bool _isFreeCoinsPanel = false;
    [SerializeField] private bool _isMainMenu = false;

    private Vector3 currentPosition
    {
        get => movingObject.anchoredPosition3D;
        set { movingObject.anchoredPosition3D = value; }
    }

    private Vector3 outOfScreenPosition
    {
        get => new Vector3(0, CanvasManager.Height ,0);
    }
    public Vector3 outOfScreenPositionVIP;

    private Vector3 offsetPosition
    {
        get => _initPosition + (_initPosition - outOfScreenPosition) * movingDownOffset;
    }

    private void Awake()
    {
        if (onFadebackClick == null) return;

        button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => { onFadebackClick?.Invoke(); });

        outOfScreenPositionVIP = movingObject.position;
    }

    private void OnValidate()
    {
        if (movingObject == null) movingObject = GetComponent<RectTransform>();

        _initPosition = currentPosition;
    }

    public void Show()
    {
        
        Debug.Log("Show panel");
        IsActive = true;

        currentPosition = outOfScreenPosition;

        var tween = movingObject.DOAnchorPos3D(offsetPosition, firstDuration).OnComplete(()=> { GameManager.Instance.EnableEventSystem(); });
        tween.onKill += () => {
            var tween1 = movingObject.DOAnchorPos3D(_initPosition, secondDuration);
            tween1.onKill += () => {   onShowPre?.Invoke();  };
            
        };

        onShow?.Invoke();
        
        if(!ignoreItAsPopup)
        PopupController.Instance.activePopup = this;

        if (_playButtonSound)
            GameManager.Instance._btnClickSound.PlaySound();
        if (_popUpSound)
            GameManager.Instance._popUpSound.PlaySound();
        if (_specialOfferSound)
            GameManager.Instance._specialOfferSound.PlaySound();
    }

    public void ShowVIP()
    {
        Debug.Log("ShowVIP panel");
        IsActive = true;

        currentPosition = outOfScreenPositionVIP;
        //currentPosition = outOfScreenPosition;

        var tween = movingObject.DOAnchorPos3D(new Vector3(0,0,0), firstDuration);
        tween.onKill += () => {
            var tween1 = movingObject.DOAnchorPos3D(new Vector3(-500, 0, 0), secondDuration);
            tween1.onKill += () => { onShowPre?.Invoke(); };
        };

        onShow?.Invoke();

        if (!ignoreItAsPopup)
            PopupController.Instance.activePopup = this;
    }

    private void FadeStartTapToPlay()
    {
        GameManager.Instance.tapToPlay.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
    }
    public void Hide()
    {

        Debug.Log("Hide");
        
        currentPosition = _initPosition;

        Vector3 scale = Vector3.one;
        Vector3 scale2 = Vector3.one;

        if (_isMainMenu)
        {
            scale = GameManager.Instance.mainSawObj.transform.localScale;
            GameManager.Instance.mainSawObj.transform.localScale = Vector3.zero;

            scale2 = GameManager.Instance.mainTimerObj.transform.localScale;
            GameManager.Instance.mainTimerObj.transform.localScale = Vector3.zero;

            GameManager.Instance.tapToPlay.GetComponent<CanvasGroup>().alpha = 0;

            Invoke("FadeStartTapToPlay", 0.3f);
        }

        var tween = movingObject.DOAnchorPos3D(offsetPosition, secondDuration);
        tween.onKill += () =>
        {

            var tween2 = movingObject.DOAnchorPos3D(outOfScreenPosition, firstDuration).OnComplete(() => { 
                GameManager.Instance.EnableEventSystem();
                if (_isMainMenu)
                {
                    GameManager.Instance.mainSawObj.transform.DOScale(scale, 0.1f);

                    GameManager.Instance.mainTimerObj.transform.DOScale(scale2, 0.1f);
                }
            });

            tween2.onKill += () =>
            {
                if (!_isFreeCoinsPanel)
                {
                    
                    onHide?.Invoke();
                }
                    
                IsActive = false;
            };
        };

        if (PopupController.Instance.activePopup == this && !ignoreItAsPopup)
            PopupController.Instance.activePopup = null;

        if (!_isFreeCoinsPanel)
        {
            
            onHideFinalize?.Invoke();
        }
            

        if (_playButtonSound)
            GameManager.Instance._btnClickSound.PlaySound();

    }

    public void HideImmediate()
    {
        IsActive = false;
        
        currentPosition = outOfScreenPosition;
        if (!_isFreeCoinsPanel)
        {
            //Debug.LogError("-----Hide_Test2");
            onHide?.Invoke();
            onHideFinalize?.Invoke();
        }

        if (PopupController.Instance.activePopup == this && !ignoreItAsPopup)
            PopupController.Instance.activePopup = null;

    }

    public void ShowImmediate()
    {
        IsActive = true;

        onShowPre?.Invoke();
        currentPosition = _initPosition;
        onShow?.Invoke();

        if (!ignoreItAsPopup)
            PopupController.Instance.activePopup = this;
    }


}
