using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI, helpMenuUI;
    [SerializeField] private TextMeshProUGUI highScoreText;
    private bool isHelpMenuOpen = false;

    private void Start()
    {
        UpdateHighScoreText();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    private void UpdateHighScoreText()
    {
        if (HighScoreTracker.Instance != null)
        {
            highScoreText.text = "High Score\nis\n" + HighScoreTracker.Instance.HighScore + "!";
        }
        else
        {
            Debug.LogWarning("Can't find HighScoreTracker");
        }
    }

    public void HelpMenu()
    {
        isHelpMenuOpen = !isHelpMenuOpen;

        mainMenuUI.SetActive(!isHelpMenuOpen);
        helpMenuUI.SetActive(isHelpMenuOpen);
    }

    public void ResetHighScore()
    {
        HighScoreTracker hsTracker = FindAnyObjectByType<HighScoreTracker>();

        if (hsTracker != null)
        {
            hsTracker.ResetHighScore();
            UpdateHighScoreText();
        }
    }
}
