using UnityEngine;
using UnityEngine.UI;
using Terresquall;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform topBound, bottomBound, bulletSpawn;
    [SerializeField] private GameObject playerExplosionPrefab, shieldPrefab;
    [SerializeField] private Animator animator;
    [SerializeField] private Slider healthBar;
    [SerializeField] private float turnSpeed = 4.0f, shotSpeed = 3f, shotTimer = 1f, moveSpeed = 2.0f;
    [SerializeField] private int health = 10, shieldHealth = 3;
    [SerializeField] Image[] shieldHealthIcons;
    [System.NonSerialized] public bool isPlayerAlive = true;
    private int currentHealth, currentShieldHealth;
    ObjectManager objManager;
    GameManager gameManager;
    EnemyDetection enemyDetection;
    Vector2 movement;
    Rigidbody2D rb;
    private Camera cam;
    private float halfWidth, turn;
    private bool shieldActive = false;
    [System.NonSerialized] public float timer;

    private void Awake()
    {
        cam = Camera.main;
        halfWidth = GetComponent<Collider2D>().bounds.extents.x;
        rb = GetComponent<Rigidbody2D>();
        objManager = FindAnyObjectByType<ObjectManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        enemyDetection = FindAnyObjectByType<EnemyDetection>();
        timer = 0f;
        currentHealth = health;
        currentShieldHealth = shieldHealth;
        healthBar.maxValue = health;
        healthBar.value = currentHealth;
        gameObject.SetActive(true);
    }

    void Update()
    {
        float moveX = VirtualJoystick.GetAxis("Horizontal", 1);
        float moveY = VirtualJoystick.GetAxis("Vertical", 1);

        movement = new Vector2(moveX, moveY).normalized;

        CheckBounds();

        turn = Mathf.MoveTowards(turn, moveX, turnSpeed * Time.deltaTime);
        animator.SetFloat("BankTime", (turn + 1.0f) / 2.0f);


        if (enemyDetection.canShoot)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                Shoot();
            }
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    private void CheckBounds()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, bottomBound.position.y, topBound.position.y);
        transform.position = pos;

        float left = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        float right = cam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;

        pos.x = Mathf.Clamp(pos.x, left + halfWidth, right - halfWidth);
        transform.position = pos;
    }

    private void Shoot()
    {
        if (gameManager.isPaused) return;

        GameObject playerBullet = objManager.GetObject(ObjectManager.ObjectType.PlayerBullet);
        playerBullet.transform.position = bulletSpawn.transform.position;
        
        Rigidbody2D playerBulletRb = playerBullet.GetComponent<Rigidbody2D>();
        playerBulletRb.linearVelocity = new Vector2(0f, shotSpeed);

        timer = shotTimer;
    }

    public void TakeDamage(int damage)
    {
        if (shieldActive)
        {
            currentShieldHealth--;

            if (currentShieldHealth >= 0 && currentShieldHealth < shieldHealthIcons.Length)
            {
                shieldHealthIcons[currentShieldHealth].gameObject.SetActive(false);
            }

            if (currentShieldHealth <= 0)
            {
                shieldActive = false;
                shieldPrefab.SetActive(false);
            }

            return;
        }   

        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            isPlayerAlive = false;
            gameObject.SetActive(false);
            GameObject playerExplosionInst = Instantiate(playerExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(playerExplosionInst, 0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shield"))
        {
            shieldActive = true;
            currentShieldHealth = shieldHealth;
            shieldPrefab.SetActive(true);
            for (int i = 0; i < shieldHealthIcons.Length; i++)
            {
                shieldHealthIcons[i].gameObject.SetActive(true);
            }

            Destroy(collision.gameObject);
            gameManager.randomPickup = null;
        }
        if (collision.CompareTag("RapidFire"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(RapidFire());
            gameManager.randomPickup = null;
        }
        if (collision.CompareTag("Health"))
        {
            currentHealth += 2;
            if (currentHealth > health) currentHealth = health;
            healthBar.value = currentHealth;
            Destroy(collision.gameObject);
            gameManager.randomPickup = null;
        }
    }

    IEnumerator RapidFire()
    {
        shotTimer = shotTimer / 2;

        yield return new WaitForSeconds(10.0f);

        shotTimer = shotTimer * 2;
    }
}
