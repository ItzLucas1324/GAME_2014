using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject gameUI, pauseMenuUI, gameOverUI, pauseButtonParent;
    [SerializeField] private float offscreenMargin = 1f, pickupDropSpeed;
    [SerializeField] private Vector2 pickupSpawnTimes = new Vector2(5f, 10f);
    [System.NonSerialized] public int score, finalScore;
    [System.NonSerialized] public bool isPaused, isGameOver;
    [System.NonSerialized] public GameObject randomPickup = null;
    private float topBound, bottomBound, leftBound, rightBound, timer;
    public TextMeshProUGUI finalScoreTextValue;
    PlayerController playerController;
    ObjectManager objManager;
    Camera cam;

    void Start()
    {
        Time.timeScale = 1;
        playerController = FindAnyObjectByType<PlayerController>();
        objManager = FindAnyObjectByType<ObjectManager>();

        gameUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        isPaused = false;
        isGameOver = false;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic();
            SoundManager.Instance.PlayMusic("The Evil Sacrifice Archenemies");
        }

        cam = Camera.main;
        topBound = cam.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y + offscreenMargin;
        bottomBound = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y - offscreenMargin;
        leftBound = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x + offscreenMargin;
        rightBound = cam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x - offscreenMargin;

        timer = Random.Range(pickupSpawnTimes.x, pickupSpawnTimes.y + 1);
        score = 0;
    }

    private static ObjectManager.ObjectType[] pickupTypes =
    {
        ObjectManager.ObjectType.ShieldPickup,
        ObjectManager.ObjectType.RapidFirePickup,
        ObjectManager.ObjectType.HealthPickup
    };

    private void Update()
    {
        if (randomPickup == null)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SpawnPickups();
                timer = Random.Range(pickupSpawnTimes.x, pickupSpawnTimes.y + 1) + 5f;
            }
        }
        else if (randomPickup.transform.position.y <= bottomBound)
        {
            Destroy(randomPickup);
            randomPickup = null;
        }
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

        if (isPaused)
            PauseGame();
    }

    public void QuitGame()
    {
        SoundManager.Instance.StopMusic();
        SceneManager.LoadScene("MainMenu");
        PauseGame();
    }

    public void TriggerGameOver()
    {
        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        SoundManager.Instance.StopMusic();
        finalScore = score;
        finalScoreTextValue.text = finalScore.ToString();
        gameUI.SetActive(false);

        if (HighScoreTracker.Instance != null)
            HighScoreTracker.Instance.SubmitScore(finalScore);
        else
            Debug.LogWarning("No HighScoreTracker found!");

        yield return new WaitForSeconds(1.5f);

        gameOverUI.SetActive(true);
        isGameOver = true;
    }

    private void SpawnPickups()
    {
        int pickupIndex = Random.Range(0, pickupTypes.Length);
        ObjectManager.ObjectType randomPickupType = pickupTypes[pickupIndex];

        float spawnPosX = Random.Range(leftBound, rightBound);
        randomPickup = objManager.ObjectFactory(randomPickupType);

        randomPickup.transform.position = new Vector2(spawnPosX, topBound);
        Rigidbody2D rb = randomPickup.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(0f, -pickupDropSpeed);
    }
}
