using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TaskBlock : MonoBehaviour
{
    [SerializeField] private Image _fillBar;
    [SerializeField] private TextMeshProUGUI _value, _compliteText;
    [SerializeField] private GameObject _compliteGroup, _compliteImage, _coins;

    [SerializeField] private DailyGoalsAnimation _dailyGoalsAnimation;

    private Transform _coinTransform;

    private bool _state;
    private bool _animationState;

    private void Awake()
    {
        _coinTransform = _coins.GetComponent<Transform>();
    }

    public void Complite()
    {
        if (_state == false)
        {

            _compliteImage.SetActive(true);

            _compliteText.text = "complite";

            _state = true;
        }
    }

    public void NoComplite()
    {
        _compliteGroup.SetActive(true);
        _compliteImage.SetActive(false);

        _compliteText.text = "reward:";
    }

    public void Value(string value)
    {
        _value.text = value;
    }

    public void FillBar(float fill)
    {
        _fillBar.fillAmount = fill;
    }

    public void SetActiveGameObject()
    {
        gameObject.SetActive(false);
    }

    public void StartAnimations(Transform coinAcceptor, float timeAnim = 1)
    {
        if (_state == true && _animationState == false)
        {
            _coinTransform.DOMove(new Vector3(coinAcceptor.position.x, coinAcceptor.position.y), timeAnim).onComplete += () => 
            {
                _compliteGroup.SetActive(false);
                _dailyGoalsAnimation.UpLine();
            };
            _animationState = true;
        }
    }
}
