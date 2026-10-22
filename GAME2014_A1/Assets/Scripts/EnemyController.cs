using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f, swingLength, swingSpeed;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] Vector2 shotTimerRange = new Vector2(1, 3);
    private float timer, shotTimer;
    Rigidbody2D rb;
    private bool canSwing;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        int swingOdds = Random.Range(0, 2);

        if (swingOdds == 1)
            canSwing = true;

        if (canSwing)
            timer = Random.Range(0f, Mathf.PI) / swingSpeed;

        shotTimer = Random.Range(shotTimerRange.x, shotTimerRange.y);
    }

    private void FixedUpdate()
    {
        shotTimer -= Time.fixedDeltaTime;

        if (shotTimer <= 0)
        {
            Shoot();
        }

        float x = 0f;

        if (canSwing)
        {
            timer += Time.fixedDeltaTime;
            x = Mathf.Cos(timer * swingSpeed) * swingLength * swingSpeed;
        }

        rb.linearVelocity = new Vector2(x, -moveSpeed);
    }

    private void Shoot()
    {

    }
}
