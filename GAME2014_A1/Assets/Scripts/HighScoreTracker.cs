using UnityEngine;

public class HighScoreTracker : MonoBehaviour
{
    public static HighScoreTracker Instance;
    private const string HighScoreKey = "HighScore";
    public int HighScore { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public bool SubmitScore(int score)
    {
        if (score <= HighScore)
        {
            return false;
        }

        HighScore = score;
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
        return true;
    }

    public void ResetHighScore()
    {
        HighScore = 0;
        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.Save();
    }
}
