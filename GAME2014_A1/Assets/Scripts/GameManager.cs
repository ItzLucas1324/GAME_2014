using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject gameUI, pauseMenuUI, pauseButtonParent;
    [System.NonSerialized] public int score;
    [System.NonSerialized] public bool isPaused;
    PlayerController playerController;

    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();

        gameUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        isPaused = false;

        score = 0;
    }

    public void SetScore(int scoreValue)
    {
        if (!playerController.isPlayerAlive)
        {
            return;
        }

        score += scoreValue;

        if (score <= 0)
        {
            score = 0;
        }

        scoreText.text = score.ToString();
    }

    public void PauseGame()
    {
        isPaused = !isPaused;

        Time.timeScale = 1;

        if (isPaused)
        {
            Time.timeScale = 0;
        }

        gameUI.SetActive(!isPaused);
        pauseMenuUI.SetActive(isPaused);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
        PauseGame();
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
        PauseGame();
    }
}
