using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f, ledgeCheckDistance = 0.5f, playerAgroDistance = 5f, stopDistance = 1f, hitStunTime = 0.5f, forceKnockbackSpeed = 6f, meleeCooldown = 1f, spriteFadeRate = 1f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] GameObject projectilePrefab, meleeHitBox;
    [SerializeField] Transform projectileSpawn;
    private float direction = 1f, stopTimer, shootTimer, hitTimer, meleeTimer;
    private int currentHealth;
    private bool isPlayerInSight = false, wasGrounded = true, isDead = false;
    Rigidbody2D rb;
    Collider2D col;
    Animator an;
    PlayerController player;
    SpriteRenderer sr;
    ContactFilter2D groundFilter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        an = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        player = FindAnyObjectByType<PlayerController>();
        groundFilter.SetNormalAngle(80f, 100f);

        currentHealth = maxHealth;
        meleeHitBox.SetActive(false);

        stopTimer = Random.Range(0.2f, 1f);
        shootTimer = Random.Range(2f, 4f);
    }

    private void Update()
    {
        float playerDistance = Vector2.Distance(player.transform.position, transform.position);
        isPlayerInSight = playerDistance <= playerAgroDistance;

        an.SetBool("IsDead", isDead);
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        if (hitTimer > 0)
        {
            hitTimer -= Time.fixedDeltaTime;
        }

        if (meleeTimer > 0)
        {
            meleeTimer -= Time.fixedDeltaTime;
        }

        bool isGrounded = rb.IsTouching(groundFilter);
        bool inForcePull = an.GetCurrentAnimatorStateInfo(0).IsName("CloneForcePull");
        bool inForcePush = an.GetCurrentAnimatorStateInfo(0).IsName("CloneForcePush");
        bool inEnemyFall = an.GetCurrentAnimatorStateInfo(0).IsName("CloneFall");

        if (isGrounded && !wasGrounded)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (inForcePull)
            {
                an.Play("CloneIdle");
                inForcePull = false;
                hitTimer = hitStunTime;
            }

            if (inForcePush)
            {
                an.Play("CloneFall");
                inForcePush = false;
                hitTimer = hitStunTime;
            }
        }

        wasGrounded = isGrounded;

        bool isHit = hitTimer > 0 || inForcePull || inEnemyFall;

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

            if (Mathf.Abs(target) <= stopDistance)
            {
                bool inMelee = an.GetCurrentAnimatorStateInfo(0).IsName("CloneMelee");

                if (!isHit && !inMelee && meleeTimer <= 0)
                {
                    an.Play("CloneMelee");
                    meleeTimer = meleeCooldown;
                }
            }
            else if (stopTimer > 0)
            {
                bool inShoot = an.GetCurrentAnimatorStateInfo(0).IsName("CloneShooting");

                if (!inShoot && !isHit)
                {
                    stopTimer -= Time.fixedDeltaTime;

                    if (groundAhead)
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
        an.SetBool("IsShooting", isPlayerInSight && stopTimer <= 0 && !isHit && Mathf.Abs(target) > stopDistance);

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
        if (isDead)
            return;

        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            meleeHitBox.SetActive(false);
            hitTimer = hitStunTime;
            TakeDamage(player.currentDamage);
            an.Play("CloneIdle");
        }

        if (collision.gameObject.CompareTag("PlayerForce"))
        {
            meleeHitBox.SetActive(false);

            ForceScript force = collision.gameObject.GetComponent<ForceScript>();

            float awayFromPlayer = Mathf.Sign(transform.position.x - player.transform.position.x);

            if (force.mode == ForceScript.ForceMode.ForcePush)
            {
                rb.linearVelocity = new Vector2(awayFromPlayer * forceKnockbackSpeed, 5f);
                an.Play("CloneForcePush");
            }
            else if (force.mode == ForceScript.ForceMode.ForcePull)
            {
                rb.linearVelocity = new Vector2(-awayFromPlayer * forceKnockbackSpeed, 5f);
                an.Play("CloneForcePull");
            }
            else
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                an.Play("CloneFall");
            }

            hitTimer = hitStunTime;
            TakeDamage(force.damage);
        }
    }

    public void TakeDamage(int amount)
    {
        meleeHitBox.SetActive(false);

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            isDead = true;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        //Debug.Log("Damage Taken: " + currentHealth);
    }

    public void Shoot()
    {
        GameObject projectileInst = Instantiate(projectilePrefab, projectileSpawn.transform.position, Quaternion.identity);
        projectileInst.GetComponent<EnemyProjectileScript>().Init(direction);
    }

    public void Dead()
    {
        rb.simulated = false;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1.5f);

        Color color = sr.color;

        while (color.a > 0f)
        {
            color.a = Mathf.MoveTowards(color.a, 0f, spriteFadeRate * Time.deltaTime);
            sr.color = color;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void EnableMeleeCollider()
    {
        meleeHitBox.SetActive(true);
    }

    public void DisableMeleeCollider()
    {
        meleeHitBox.SetActive(false);
    }
}
