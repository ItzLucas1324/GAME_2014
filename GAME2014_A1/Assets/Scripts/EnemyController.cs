using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f, swingLength, swingSpeed, shotSpeed = 4.0f, turnSpeed = 8.0f;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] Vector2 shotTimerRange = new Vector2(1, 3);
    [SerializeField] private float offscreenMargin = 1f;
    [SerializeField] private int health = 2;
    private int currentHealth;
    private float timer, shotTimer, bottomBound, turnInput, turn;
    Rigidbody2D rb;
    private bool canSwing;
    ObjectManager objManager;
    GameManager gameManager;
    EnemySpawner spawner;
    Camera cam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        objManager = FindAnyObjectByType<ObjectManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        spawner = FindAnyObjectByType<EnemySpawner>();

        cam = Camera.main;
        bottomBound = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y - offscreenMargin;
    }

    void OnEnable()
    {
        timer = 0f;
        turn = 0f;
        turnInput = 0f;

        currentHealth = health;

        int swingOdds = Random.Range(0, 2);

        if (swingOdds == 1)
            canSwing = true;

        if (canSwing)
            timer = Random.Range(0f, Mathf.PI) / swingSpeed;

        shotTimer = Random.Range(shotTimerRange.x, shotTimerRange.y);
    }

    private void FixedUpdate()
    {
        if (transform.position.y <= bottomBound)
        {
            objManager.ReturnObject(ObjectManager.ObjectType.Enemy, gameObject);
            gameManager.SetScore(-1);
            spawner.currentEnemyCount--;
            return;
        }

        shotTimer -= Time.fixedDeltaTime;

        if (shotTimer <= 0)
        {
            Shoot();
        }

        float x = 0f;
        turnInput = 0f;

        if (canSwing)
        {
            timer += Time.fixedDeltaTime;
            float wave = Mathf.Cos(timer * swingSpeed);

            x = wave * swingLength * swingSpeed;
            turnInput = wave;
        }

        rb.linearVelocity = new Vector2(x, -moveSpeed);
    }

    private void Update()
    {
        turn = Mathf.MoveTowards(turn, turnInput, turnSpeed * Time.deltaTime);
        animator.SetFloat("BankTime", (turn + 1f) / 2f);
    }

    private void Shoot()
    {
        if (objManager != null)
        {
            GameObject enemyBullet = objManager.GetObject(ObjectManager.ObjectType.EnemyBullet);
            enemyBullet.transform.position = bulletSpawn.transform.position;
            enemyBullet.transform.rotation = Quaternion.Euler(0f, 0f, 180f);

            Rigidbody2D enemyRb = enemyBullet.GetComponent<Rigidbody2D>();
            enemyRb.linearVelocity = new Vector2(0f, -shotSpeed);

            shotTimer = Random.Range(shotTimerRange.x, shotTimerRange.y);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            objManager.ReturnObject(ObjectManager.ObjectType.Enemy, gameObject);

            float randomRotation = Random.Range(0f, 360f);
            GameObject explosionInst = Instantiate(explosionPrefab, transform.position, Quaternion.Euler(0f, 0f, randomRotation));
            Destroy(explosionInst, 0.55f);

            spawner.currentEnemyCount--;
            gameManager.SetScore(2);
        }
    }
}
