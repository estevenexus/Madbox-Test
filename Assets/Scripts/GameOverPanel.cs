using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public Text ScoreText;

    public void SetScoreText(int lastScore)
    {
        ScoreText.text = $"SCORE: {lastScore}\nHIGH SCORE: {PlayerPrefs.GetInt("HighScore", 0)}";
    }
}
