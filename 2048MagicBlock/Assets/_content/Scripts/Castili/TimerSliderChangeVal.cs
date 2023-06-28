using UnityEngine;
using UnityEngine.UI;

public class TimerSliderChangeVal : MonoBehaviour
{
    [SerializeField] Slider _timerSlider;
    public void TimerSliderChangeValCheck()
    {
        if (GameManager.Instance.isGameOverScreen && _timerSlider.value != 0)
        {
            _timerSlider.value = 0;
        }
    }
}
