using UnityEngine;
using Terresquall;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform topBound, bottomBound, bulletSpawn;
    [SerializeField] private Animator animator;
    [SerializeField] private float turnSpeed = 4.0f, shotSpeed = 3f, shotTimer = 1f, moveSpeed = 2.0f;
    ObjectManager manager;
    EnemyDetection enemyDetection;
    Vector2 movement;
    Rigidbody2D rb;
    private Camera cam;
    private float halfWidth, turn;
    [System.NonSerialized] public float timer;

    private void Awake()
    {
        cam = Camera.main;
        halfWidth = GetComponent<Collider2D>().bounds.extents.x;
        rb = GetComponent<Rigidbody2D>();
        manager = FindAnyObjectByType<ObjectManager>();
        enemyDetection = FindAnyObjectByType<EnemyDetection>();
        timer = 0f;
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
        GameObject playerBullet = manager.GetObject(ObjectManager.ObjectType.PlayerBullet);
        playerBullet.transform.position = bulletSpawn.transform.position;
        
        Rigidbody2D playerBulletRb = playerBullet.GetComponent<Rigidbody2D>();
        playerBulletRb.linearVelocity = new Vector2(0f, shotSpeed);

        timer = shotTimer;
    }
}
