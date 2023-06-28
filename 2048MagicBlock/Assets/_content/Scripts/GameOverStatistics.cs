using TMPro;
using UnityEngine;

public class GameOverStatistics : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI highScoreText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI maxValueCounterText;

    private void OnEnable()
    {
        scoreText.text = GameManager.Instance.Score.ToString();
        highScoreText.text = PlayerPrefs.GetInt("highScore1", 0).ToString();//GameData.highScore.ToString();
        maxValueCounterText.text = $"x {CellLogic.Instance.MaxValueCounter}";
    }


}

