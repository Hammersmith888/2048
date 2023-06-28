using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Chest : MonoBehaviour
{
    [SerializeField] private int _reward;
    [SerializeField] private Sprite _openSprite, _closeSprite;
    [SerializeField] private Image _cheshImage;
    [SerializeField] private GameObject _shineImage;
    private Button _button;
    public bool canOpen=true;
    private bool _isOpened;
    private CollectAnimationChest _collectAnimationChest;
    private void Awake()
    {
       // _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OpenChest);
    }

    public void SetCollectAnimationChest(CollectAnimationChest collector)
    {
        _collectAnimationChest = collector;
    }
    public void OpenChest()
    {
        if(canOpen)
        {
            canOpen = false;
            _isOpened = true;
            AnimationChest();
            GameManager.Instance.DisableEventSystem();
        }
    }
    public void SetIsOpenedChest(bool isOpened)
    {
        _isOpened = isOpened;
    }
    public void ChangeStateToOpened(bool state)
    {
        canOpen = state;
    }
    public void ChangeSprite()
    {
        Sprite sprite = _isOpened ? _openSprite : _closeSprite;
        _cheshImage.sprite=sprite; ;
    }
    private void StartAnimationCollectCoins()
    {
        _collectAnimationChest.SetStartPosition(GetComponent<RectTransform>());
        _collectAnimationChest.StartAnimation(
            (FindObjectOfType<DailyGoalsAnimation>().IndexSelectedChest + 1) * 10
            );
        FindObjectOfType<DailyGoalsAnimation>().CollectReward(FindObjectOfType<DailyGoalsAnimation>().IndexSelectedChest);
        GetComponent<RotationChestAnimation>().inRotationMode = false;
        GameManager.Instance._rewardSound.PlaySound();
    }
    
    public void AnimationChest()
    {
        transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 1f).OnComplete(() => { GameManager.Instance.EnableEventSystem(); ChangeSprite(); StartAnimationCollectCoins(); });
        
    }
}
