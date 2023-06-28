using System.Collections;
using UnityEngine;

public class RewardedAnimController : MonoBehaviour
{
    [SerializeField] private AnimationCollectCoins _animationCollectCoins;
    [SerializeField] private OpenAnimation _openAnimation;

    public void Claim()
    {
        //Debug.LogError("------RewardedAnimController--Claim()");
        _openAnimation.CloseWindiw();
        StartCoroutine(AnimationWaiter());
        IEnumerator AnimationWaiter()
        {
            //Debug.LogError("------RewardedAnimController--Claim()2");
            GetCoinsWithAnimation();

            yield return new WaitUntil(() => !_animationCollectCoins.isActiveAnimation);

            GameManager.Instance.gameData.SaveData();
            GameManager.Instance.HideBlackBG();
            //Debug.LogError("------RewardedAnimController--Claim()3--this.gameObject.SetActive(false);");
            this.gameObject.SetActive(false);
        }
    }

    private void GetCoinsWithAnimation()
    {
        _animationCollectCoins.SetStartPosition(transform.GetComponent<RectTransform>());
        _animationCollectCoins.StartAnimation(3 * 5);
    }
}
