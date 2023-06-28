using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DayInfo
{
    public Image instance;
    public int value;
}

//TODO: добавить функциональность
public class DailyRewardPanel : AnimationPanel
{
    [SerializeField] private AnimationCollectCoins _animationCollectCoins;
    [SerializeField] private GameObject[] _dailyCompletedObj;

    public Sprite DayTodaySmall;
    public Sprite DayTodayLarge;

    public DayInfo[] smallDays;
    public DayInfo largeDay7;

    private int _numberDay;
    public int NumberDay
    {
        get { return _numberDay; }
        private set
        {
            int day = value;
            while (day > 6)
            {
                day -= 6;
            }
            _numberDay = day;
        }
    }


    public void CheckDaily()
    {
        if (Saver.Instance.PlayerData.lastVisitData != DateTime.Today.Day)
        {
            NumberDay = Saver.Instance.PlayerData.numberDayInReward;

            for (int i = 0; i < NumberDay; i++)
            {
                _dailyCompletedObj[i].SetActive(true);
            }

            Debug.Log(NumberDay);
            Saver.Instance.AddDayToDailyReward();
            GameData.dailyRewardDay = 1;
            GameManager.Instance.gameData.SaveData();
            UpdateInfo();
        }
    }

    private void UpdateInfo()
    {
        if (NumberDay != 6)
        {
            smallDays[NumberDay].instance.sprite = DayTodaySmall;
        }
        else
        {
            largeDay7.instance.sprite = DayTodayLarge;
        }

        Show();
        GameManager.Instance.ShowBlackBG();
    }

    private void GetCoinsWithAnimation(int multiply)
    {
        if (NumberDay != 6)
        {
            _animationCollectCoins.SetStartPosition(smallDays[NumberDay].instance.GetComponent<RectTransform>());
            _animationCollectCoins.StartAnimation(smallDays[NumberDay].value * multiply);
            AnalyticsManager.Instance.LogEvent("daily_reward"+ smallDays[NumberDay].value * multiply);
        }
        else
        {
            _animationCollectCoins.SetStartPosition(largeDay7.instance.GetComponent<RectTransform>());
            _animationCollectCoins.StartAnimation(largeDay7.value * multiply);
            AnalyticsManager.Instance.LogEvent("daily_reward" + largeDay7.value * multiply);
        }
    }
    public void Claim()
    {
        StartCoroutine(AnimationWaiter());
        IEnumerator AnimationWaiter()
        {

            GetCoinsWithAnimation(1);

            yield return new WaitUntil(() => !_animationCollectCoins.isActiveAnimation);

            Hide();
            GameManager.Instance.gameData.SaveData();
            GameManager.Instance.HideBlackBG();
        }
    }

    public void ClaimX3()
    {
        AdsController.Instance.ShowRewardedInterstitialAd(() =>
        {
            StartCoroutine(AnimationWaiter());
            AnalyticsManager.Instance.LogEvent("clime_ad");
            IEnumerator AnimationWaiter()
            {

                GetCoinsWithAnimation(3);
                yield return new WaitUntil(() => !_animationCollectCoins.isActiveAnimation);

                Hide();
                GameManager.Instance.gameData.SaveData();
                GameManager.Instance.HideBlackBG();
            }
        });
        
    }

}
