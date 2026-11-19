using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameUI, gameOverParent;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GameOver()
    {
        gameUI.SetActive(false);
        gameOverParent.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
