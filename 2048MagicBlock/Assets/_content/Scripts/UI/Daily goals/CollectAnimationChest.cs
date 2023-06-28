using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectAnimationChest : MonoBehaviour
{
    [SerializeField] private float _timeToMove;
    [SerializeField] private GameObject _prefabCoins;
    [SerializeField] private GameObject _chestPrefab;
    [SerializeField] private RectTransform _centrePosition;
    [SerializeField] private RectTransform _coinsPosition;
    [SerializeField] private RectTransform _backShine;
    [SerializeField] private Image _fade;
    [SerializeField] private TextMeshProUGUI _countReward;
    [SerializeField] private MoneyStats _moneyStats;
    private RectTransform _startPosition;
    private bool rotaionNeed;
    private GameObject tempChest;

    public bool isActiveAnimation;
    private int countMoney;

    public void SetStartPosition(RectTransform postion)
    {
        _startPosition = postion;
    }
    public void StartAnimation(int count)
    {
        countMoney = count;
        MoreCoins(count);
        isActiveAnimation = true;
        tempChest = Instantiate(_chestPrefab, _startPosition.position, Quaternion.identity, this.transform);
       // tempChest.transform.DOLocalJump(_centrePosition.localPosition, 50, 1, _timeToMove).OnComplete(() => { StartCoroutine(MoreCoins(count)); });
        tempChest.transform.DOLocalMove(_centrePosition.localPosition, _timeToMove).OnComplete(() => {MoreCoins(count); });
        _fade.gameObject.SetActive(true);
        _fade.DOFade(0.9f, _timeToMove+0.5f);

    }
    private IEnumerator RotationShine()
    {
        while (rotaionNeed)
        {
            _backShine.DOLocalRotate(new Vector3(0, 0, _backShine.localEulerAngles.z - 180), 1.5f, RotateMode.Fast).SetEase(Ease.Linear);
            yield return new WaitForSeconds(1.5f);
            Debug.Log(rotaionNeed);
        }
    }

    public void GetCoin(int multiply)
    {
        if (multiply == 3)
        {
            AdsController.Instance.ShowRewardedInterstitialAd(() => { GetCoins(); });
        }
        else
        {
            GetCoins();
        }
        void GetCoins()
        {
            StartCoroutine(Waiter());
            IEnumerator Waiter()
            {
                int nextDirection = 1;
                rotaionNeed = false;
                _backShine.gameObject.SetActive(false);
                _countReward.gameObject.SetActive(false);
                FindObjectOfType<DailyGoalsAnimation>().StopAnimationChest();
                _moneyStats.SetOrderInLayer(11);
                _moneyStats.AddMoneyWithAnimation(countMoney * multiply);
                for (int i = 0; i < 10; i++)
                {
                    GameObject tempCoin1 = Instantiate(_prefabCoins, _centrePosition.position, Quaternion.identity, this.transform);
                    Vector3 nextPosition = new Vector3(Random.Range(60, 200) * nextDirection, Random.Range(100, 400), 0);
                    Vector3 position2 = new Vector3(nextPosition.x, nextPosition.y + 200);
                    Sequence quence = DOTween.Sequence();
                    quence.Append(tempCoin1.transform.DOLocalMove(nextPosition, _timeToMove / 3).SetEase(Ease.InSine));
                    quence.Append(tempCoin1.transform.DOLocalMove(position2, _timeToMove / 3).SetEase(Ease.InSine));
                    quence.Append(tempCoin1.transform.DOLocalMove(_coinsPosition.localPosition, _timeToMove / 3).SetEase(Ease.OutSine).OnComplete(() => Destroy(tempCoin1)));
                    nextDirection *= -1;
                    yield return new WaitForSeconds(0.05f);
                }

                GameManager.Instance._coinsColectSound.PlaySound();
                yield return new WaitForSeconds(0.25f);
                tempChest.transform.DOScale(0,0.2f).OnComplete(() => Destroy(tempChest));

                _moneyStats.SetOrderInLayer(1);
                
                _fade.DOFade(0f, _timeToMove).OnComplete(() => { _fade.gameObject.SetActive(false); });

                isActiveAnimation = false;
                
            }
        }
       
    }
    private void MoreCoins(int count)
    {
        _backShine.gameObject.SetActive(true);
        _countReward.text = count.ToString();
        _countReward.gameObject.SetActive(true);
        rotaionNeed = true;
        StartCoroutine(RotationShine());
    }
}
