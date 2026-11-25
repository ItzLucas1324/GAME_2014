using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameUI, gameOverParent;
    [SerializeField] Transform victoryPoint, exitDoor;
    [SerializeField] float victoryTriggerDistance = 0.5f, victoryPauseTime, victoryAnimTime = 1.5f;
    PlayerController player;

    private bool exitUnlocked = false, victoryStarted = false;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {
        if (exitUnlocked && !victoryStarted && !player.isDead)
        {
            float distance = Mathf.Abs(player.transform.position.x - victoryPoint.position.x);

            if (distance <= victoryTriggerDistance)
            {
                victoryStarted = true;
                StartCoroutine(VictorySequence());
            }
        }
    }

    public void UnlockExit()
    {
        exitUnlocked = true;
    }

    private IEnumerator VictorySequence()
    {
        player.LockControls();
        yield return new WaitForSeconds(victoryPauseTime);

        exitDoor.GetComponent<Animator>().Play("ExitDoorOpen");
        yield return new WaitForSeconds(0.6f);
        player.PlayerVictory();
        yield return new WaitForSeconds(victoryAnimTime);

        float direct = Mathf.Sign(exitDoor.position.x - player.transform.position.x);
        player.AutoRun(direct);

        while ((exitDoor.position.x - player.transform.position.x) * direct > 0)
        {
            yield return null;
        }

        player.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        LevelComplete();
    }

    private void LevelComplete()
    {
        gameUI.SetActive(false);
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
