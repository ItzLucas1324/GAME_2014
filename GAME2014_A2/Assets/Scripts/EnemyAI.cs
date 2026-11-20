using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f, ledgeCheckDistance = 0.5f, playerAgroDistance = 5f, stopDistance = 1f, hitStunTime = 0.5f, forceKnockbackSpeed = 6f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private LayerMask groundLayer;
    private float direction = 1f, stopTimer, shootTimer, hitTimer;
    private int currentHealth;
    private bool isPlayerInSight = false, wasGrounded = true;
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

        currentHealth = maxHealth;

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
        if (hitTimer > 0)
        {
            hitTimer -= Time.fixedDeltaTime;
        }

        bool isGrounded = rb.IsTouching(groundFilter);
        bool inForcePull = an.GetCurrentAnimatorStateInfo(0).IsName("CloneForcePull");

        if (isGrounded && !wasGrounded)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (inForcePull)
            {
                an.Play("CloneIdle");
                inForcePull = false;
                hitTimer = hitStunTime;
            }
        }

        wasGrounded = isGrounded;

        bool isHit = hitTimer > 0 || inForcePull;

        float target = player.transform.position.x - transform.position.x;

        if (!isHit)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

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

                if (!inShoot && !isHit)
                {
                    stopTimer -= Time.fixedDeltaTime;

                    if (Mathf.Abs(target) > stopDistance && groundAhead)
                    {
                        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
                    }
                }
            }
            else
            {
                if (!isHit)
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
        }

        an.SetBool("IsWalking", rb.linearVelocity.x != 0 && !isHit);
        an.SetBool("IsShooting", isPlayerInSight && stopTimer <= 0 && !isHit);

        if (direction < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            hitTimer = hitStunTime;
            TakeDamage(player.currentDamage);
            an.Play("CloneIdle");
        }

        if (collision.gameObject.CompareTag("PlayerForce"))
        {
            ForceScript force = collision.gameObject.GetComponent<ForceScript>();

            float awayFromPlayer = Mathf.Sign(transform.position.x - player.transform.position.x);

            if (force.mode == ForceScript.ForceMode.ForcePush)
            {
                rb.linearVelocity = new Vector2(awayFromPlayer * forceKnockbackSpeed, 5f);
            }
            else if (force.mode == ForceScript.ForceMode.ForcePull)
            {
                rb.linearVelocity = new Vector2(-awayFromPlayer * forceKnockbackSpeed, 5f);
                an.Play("CloneForcePull");
            }
            else
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }

            hitTimer = hitStunTime;
            TakeDamage(force.damage);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Dead();
        }

        Debug.Log("Damage Taken: " + currentHealth);
    }

    private void Dead()
    {
        Destroy(gameObject);
    }
}
