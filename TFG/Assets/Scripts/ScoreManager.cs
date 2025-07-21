using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public Transform player;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private float highestY;

    void Start()
    {
        highestY = player.position.y;
        highScoreText.text = "Record: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    void Update()
    {
        if (player.position.y > highestY)
        {
            highestY = player.position.y;
            int score = Mathf.FloorToInt(highestY);
            scoreText.text = "Puntaje: " + score.ToString();

            if (score > PlayerPrefs.GetInt("HighScore", 0))
            {
                PlayerPrefs.SetInt("HighScore", score);
                highScoreText.text = "Record: " + score.ToString();
            }
        }
    }
}
