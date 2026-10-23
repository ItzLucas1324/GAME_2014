using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [System.NonSerialized] public int score;
    PlayerController playerController;

    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();

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
}
