using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f, swingLength, swingSpeed, shotSpeed = 4.0f, turnSpeed = 8.0f;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private Animator animator;
    [SerializeField] Vector2 shotTimerRange = new Vector2(1, 3);
    [SerializeField] private float offscreenMargin = 1f;
    private float timer, shotTimer, bottomBound, turnInput, turn;
    Rigidbody2D rb;
    private bool canSwing;
    ObjectManager manager;
    Camera cam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        manager = FindAnyObjectByType<ObjectManager>();

        cam = Camera.main;
        bottomBound = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y - offscreenMargin;
    }

    void OnEnable()
    {
        timer = 0f;
        turn = 0f;
        turnInput = 0f;

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
            manager.ReturnObject(ObjectManager.ObjectType.Enemy, gameObject);
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
        if (manager != null)
        {
            GameObject enemyBullet = manager.GetObject(ObjectManager.ObjectType.EnemyBullet);
            enemyBullet.transform.position = bulletSpawn.transform.position;
            enemyBullet.transform.rotation = Quaternion.Euler(0f, 0f, 180f);

            Rigidbody2D enemyRb = enemyBullet.GetComponent<Rigidbody2D>();
            enemyRb.linearVelocity = new Vector2(0f, -shotSpeed);

            shotTimer = Random.Range(shotTimerRange.x, shotTimerRange.y);
        }
    }
}
