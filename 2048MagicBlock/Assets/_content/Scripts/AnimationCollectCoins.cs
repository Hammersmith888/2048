using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimationCollectCoins : MonoBehaviour
{
    [SerializeField] private bool _needShowMoneyStat = true;
    [SerializeField] private bool _disableEventSystem = true;
    [SerializeField] private float _timeToMove;
    [SerializeField] private GameObject _prefabCoins;
    [SerializeField] private RectTransform _centrePosition;
    [SerializeField] private RectTransform _coinsPosition;
    [SerializeField] private RectTransform _backShine;
    [SerializeField] private Image _fade;
    [SerializeField] private TextMeshProUGUI _countReward;
    [SerializeField] private MoneyStats _moneyStats;
    private RectTransform _startPosition;
    private bool rotaionNeed;
    private GameObject tempCoin;

    public bool isActiveAnimation;
  
    public void SetStartPosition(RectTransform postion)
    {
        _startPosition = postion;
    }
    public void StartAnimation(int count)
    {
        isActiveAnimation = true;
        tempCoin = Instantiate(_prefabCoins, _startPosition.position, Quaternion.identity, this.transform);
        tempCoin.transform.DOLocalMove(_centrePosition.localPosition, _timeToMove).OnComplete(() => { StartCoroutine(MoreCoins(count)); });
        _fade.gameObject.SetActive(true);
        _fade.DOFade(0.5f, _timeToMove);

        if(_disableEventSystem)
            GameManager.Instance.DisableEventSystem();
    }
    private IEnumerator RotationShine()
    {
        while (rotaionNeed)
        {
            _backShine.DORotate(new Vector3(0, 0, transform.localEulerAngles.z - 180), 1.5f, RotateMode.Fast);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator MoreCoins(int count)
    {
        

        int nextDirection = 1;
        _backShine.gameObject.SetActive(true);
        _countReward.text = count.ToString();
        _countReward.gameObject.SetActive(true);
        rotaionNeed = true;
        StartCoroutine(RotationShine());
        yield return new WaitForSeconds(1.5f);
        
        GameManager.Instance._coinsColectSound.PlaySound(0.2f);
        
        rotaionNeed = false;
        _backShine.gameObject.SetActive(false);
        _countReward.gameObject.SetActive(false);

        if (_needShowMoneyStat)
        {
            _moneyStats.SetOrderInLayer(9);
        }
        _moneyStats.AddMoneyWithAnimation(count);
        
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

        if (_needShowMoneyStat)
        {
            _moneyStats.SetOrderInLayer(1);
        }
        Destroy(tempCoin);
        _fade.DOFade(0f, _timeToMove).OnComplete(() => { _fade.gameObject.SetActive(false); });
        rotaionNeed = false;
        isActiveAnimation = false;

        if (_disableEventSystem)
            GameManager.Instance.EnableEventSystem();
    }

}
