using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f, ledgeCheckDistance = 0.5f, playerAgroDistance = 5f, stopDistance = 1f;
    [SerializeField] private LayerMask groundLayer;
    private float direction = 1f, stopTimer, shootTimer;
    private bool isPlayerInSight = false;
    Rigidbody2D rb;
    Collider2D col;
    Animator an;
    PlayerController player;
    ContactFilter2D groundFilter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        an = GetComponent<Animator>();
        player = FindAnyObjectByType<PlayerController>();
        groundFilter.SetNormalAngle(80f, 100f);

        stopTimer = Random.Range(0.2f, 1f);
        shootTimer = Random.Range(2f, 4f);
    }

    private void Update()
    {
        float playerDistance = Vector2.Distance(player.transform.position, transform.position);
        isPlayerInSight = playerDistance <= playerAgroDistance;
    }

    void FixedUpdate()
    {
        bool isGrounded = rb.IsTouching(groundFilter);
        float target = player.transform.position.x - transform.position.x;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (isPlayerInSight)
        {
            direction = Mathf.Sign(target);

            bool groundAhead = true;

            if (isGrounded)
            {
                Bounds b = col.bounds;
                Vector2 origin = new Vector2(b.center.x + direction * b.extents.x, b.min.y + 0.05f);

                groundAhead = Physics2D.Raycast(origin, Vector2.down, ledgeCheckDistance, groundLayer);
                Debug.DrawRay(origin, Vector2.down * ledgeCheckDistance, Color.red);
            }

            if (stopTimer > 0)
            {
                bool inShoot = an.GetCurrentAnimatorStateInfo(0).IsName("CloneShooting");

                if (!inShoot)
                {
                    stopTimer -= Time.fixedDeltaTime;

                    if (Mathf.Abs(target) > stopDistance && groundAhead)
                    {
                        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
                        an.SetBool("IsWalking", true);
                    }
                }
            }
            else
            {
                an.SetBool("IsWalking", false);
                an.SetBool("IsShooting", true);

                shootTimer -= Time.fixedDeltaTime;

                if (shootTimer <= 0)
                {
                    an.SetBool("IsShooting", false);

                    stopTimer = Random.Range(0.2f, 1f);
                    shootTimer = Random.Range(2f, 4f);
                }
            }
        }

        an.SetBool("IsWalking", rb.linearVelocity.x != 0);
        an.SetBool("IsShooting", isPlayerInSight && stopTimer <= 0);

        if (direction < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
}
