using Google.Play.Review;
using System;
using System.Collections;
using UnityEngine;

public class RateUS : MonoBehaviour
{
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    [HideInInspector] public static RateUS Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowRateUSWithTimer(int timerSec)
    {
        StartCoroutine(StartTimerRateUsShow(timerSec));
    }

    private IEnumerator StartTimerRateUsShow(int timerSec)
    {
        int countTimer = 0;
        while (true)
        {
            countTimer++;
            yield return new WaitForSeconds(1);

            if (countTimer > timerSec)
            {
                CallRequestReviews();
                if(timerSec == CellLogic.Instance.balance.timerRateUsShow1)
                    PlayerPrefs.SetInt("RateUsShowed", 1);
                else if (timerSec == CellLogic.Instance.balance.timerRateUsShow2)
                    PlayerPrefs.SetInt("RateUsShowed", 2);

                break;
            }
        }

        
    }

    public void CallRequestReviews()
    {
        StartCoroutine(RequestReviews());
    }

    private IEnumerator RequestReviews()
    {
        _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            requestFlowOperation.Error.ToString();
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();


        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            requestFlowOperation.Error.ToString();
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
}
