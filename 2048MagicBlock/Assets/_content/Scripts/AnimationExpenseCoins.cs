using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimationExpenseCoins : MonoBehaviour
{
    [SerializeField] private bool _needShowMoneyStat = true;
    [SerializeField] private float _timeToMove;
    [SerializeField] private float _editAnimPosFinalY = 150;
    [SerializeField] private float _editAnimPosFinalX = 0;
    [SerializeField] private GameObject _prefabCoins; 
    [SerializeField] private RectTransform _centrePosition;
    [SerializeField] private RectTransform _finalPosition;
    [SerializeField] private RectTransform _downPosition;
    [SerializeField] private RectTransform _backShine;
    [SerializeField] private Image _fade;
    [SerializeField] private TextMeshProUGUI _countReward;
    [SerializeField] private MoneyStats _moneyStats;
    [SerializeField] private RectTransform _startPosition;
    private bool rotaionNeed;
    private GameObject tempCoin;

    public bool isActiveAnimation;

    public float GetTimeToMove()
    {
        return _timeToMove;
    }
    public void SetTimeToMove(float timeMove)
    {
        _timeToMove = timeMove;
    }
    public void SetEditAnimPosFinalY(float editAnimPosFinalY)
    {
        _editAnimPosFinalY = editAnimPosFinalY;
    }
    public void SetEditAnimPosFinalX(float editAnimPosFinalX)
    {
        _editAnimPosFinalX = editAnimPosFinalX;
    }

    public void SetFinalPosition(RectTransform postion)
    {
        _finalPosition = postion;
    }
    public void SetFinalPosition2(RectTransform postion, int count)
    {
        _finalPosition = postion;
        StartAnimation(count);
    }
    public void StartAnimation(int count = 0, bool isAd=false, bool isFortuneWheel = false)
    {
        if (!isAd)
        {
            isActiveAnimation = true;
            tempCoin = Instantiate(_prefabCoins, _startPosition.position, Quaternion.identity, this.transform);
            if(_needShowMoneyStat)
                _moneyStats.SetOrderInLayer(9);

            tempCoin.transform.DOLocalMove(_centrePosition.localPosition, _timeToMove).OnComplete(() => { StartCoroutine(MoreCoins(-count, isFortuneWheel)); });
            _fade.gameObject.SetActive(true);
            _fade.DOFade(0.5f, _timeToMove);
        }
        else
        {
            isActiveAnimation = true;
            if (_needShowMoneyStat)
                _moneyStats.SetOrderInLayer(9);
            _fade.gameObject.SetActive(false);
            _fade.DOFade(0.5f, _timeToMove);
        }
       

    }
    private IEnumerator RotationShine()
    {
        while (rotaionNeed)
        {
            _backShine.DORotate(new Vector3(0, 0, transform.localEulerAngles.z - 180), 1.0f, RotateMode.Fast);
            yield return new WaitForSeconds(0f);
        }
    }

    IEnumerator MoreCoins(int count, bool isFortuneWheel = false)
    {
        Debug.LogError("_AnimationExpenseCoins.StartAnimation(-_speenCost,true);   1");

        int nextDirection = 1;
        _backShine.gameObject.SetActive(true);
        _countReward.text = count.ToString();
        _countReward.gameObject.SetActive(true);
        rotaionNeed = true;
        StartCoroutine(RotationShine());
        yield return new WaitForSeconds(0f);

        rotaionNeed = false;
        _backShine.gameObject.SetActive(false);
        _countReward.gameObject.SetActive(false);
        if (_needShowMoneyStat)
            _moneyStats.AddMoneyWithAnimation(-count);

        if (isFortuneWheel)
        {
            Debug.LogError("_AnimationExpenseCoins.StartAnimation(-_speenCost,true);   2");
            CoinPurse.Instance.IncreaseCoins(-count);
            //_moneyStats.AddMoneyWithAnimation(-count);
        }
            

        GameManager.Instance._coinsColectSound.PlaySound();

        for (int i = 0; i < 10; i++)
        {
            GameObject tempCoin1 = Instantiate(_prefabCoins, _centrePosition.position, Quaternion.identity, this.transform);
            Vector3 nextPosition = new Vector3(Random.Range(60, 200) * nextDirection, Random.Range(100, 400), 0);
            //Vector3 position2 = new Vector3(nextPosition.x, nextPosition.y + 200);
            Sequence quence = DOTween.Sequence();
            quence.Append(tempCoin1.transform.DOLocalMove(nextPosition, _timeToMove / 2).SetEase(Ease.InSine));
            //quence.Append(tempCoin1.transform.DOLocalMove(position2, _timeToMove / 3).SetEase(Ease.InSine));
            quence.Append(tempCoin1.transform.DOLocalMove(_finalPosition.localPosition - new Vector3(_editAnimPosFinalX, _editAnimPosFinalY, 0), _timeToMove / 2).SetEase(Ease.OutSine).OnComplete(() => Destroy(tempCoin1)));


            nextDirection *= -1;
            yield return new WaitForSeconds(0.05f);
        }
        if (_needShowMoneyStat)
            _moneyStats.SetOrderInLayer(1);
        Destroy(tempCoin);
        _fade.DOFade(0f, _timeToMove).OnComplete(() => { _fade.gameObject.SetActive(false);
            //GameManager.Instance.EnableEventSystem();
        });
        rotaionNeed = false;
        isActiveAnimation = false;

        
    }
}
