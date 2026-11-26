using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameUI, gameOverParent, levelCompleteParent;
    [SerializeField] Transform victoryPoint, exitDoor;
    [SerializeField] Collider2D enemyZone;
    [SerializeField] float victoryTriggerDistance = 0.5f, victoryPauseTime, victoryAnimTime = 1.5f;
    PlayerController player;
    GeneratorOneScript generator;
    [System.NonSerialized] public bool exitUnlocked = false;
    private bool victoryStarted = false;
    Scene currentScene;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        generator = FindAnyObjectByType<GeneratorOneScript>();
        currentScene = SceneManager.GetActiveScene();
    }

    void Update()
    {
        if (exitUnlocked && !victoryStarted && !player.isDead)
        {
            float distance = Mathf.Abs(player.transform.position.x - victoryPoint.position.x);

            if (distance <= victoryTriggerDistance && !EnemiesRemaining())
            {
                generator.OpenDoor();
                victoryStarted = true;
                StartCoroutine(VictorySequence());
            }
        }

        EnemiesRemaining();
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

    public bool EnemiesRemaining()
    {
        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;

        enemyZone.Overlap(filter, results);

        foreach (Collider2D col in results)
        {
            if (col.CompareTag("Enemy"))
            {
                return true;
            }
        }

        return false; 
    }

    private void LevelComplete()
    {
        gameUI.SetActive(false);
        levelCompleteParent.SetActive(true);
    }

    public void GameOver()
    {
        gameUI.SetActive(false);
        gameOverParent.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(currentScene.buildIndex + 1);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
