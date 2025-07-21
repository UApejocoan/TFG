using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalHighScoreText;

    public void GameOver(int score)
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Puntaje: " + score;
        finalHighScoreText.text = "Record: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        Time.timeScale = 0f; // Pausa el juego
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame llamado");

        Time.timeScale = 1f; // Reactiva el juego
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
