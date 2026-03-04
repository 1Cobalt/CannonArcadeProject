using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText; // Опционально

    void Update()
    {
        if (GameManager.Instance != null)
        {
            // Форматируем красиво: "Score: 1250"
            scoreText.text = $"Score: {GameManager.Instance.score}";

            if (highScoreText != null)
                highScoreText.text = $"Best: {GameManager.Instance.highScore}";
        }
    }
}