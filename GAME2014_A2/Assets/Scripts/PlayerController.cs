using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f, jumpForce = 5.0f, sprintMulitplier = 1.8f, doubleTapWindow = 0.3f, backAttackWindow = 0.25f, upAttackWindow = 0.25f, backFlipWindow = 0.25f, parryTime = 0.3f;
    [SerializeField] private int maxHealth = 10, maxForce = 10;
    [SerializeField] GameObject attackHitBox, forceHitBox;
    [SerializeField] Slider healthBar, forceMeter;
    [SerializeField] Vector2 backAttackColliderPos;
    private float lastTapTime = -1f, lastTapDirection = 1f, lastBackTapTime = -1f, prevMoveX = 0f, lastUpTapTime = -1f, prevMoveY = 0f, speed, parryTimer, autoRunDirection = 0f;
    private Vector2 movement;
    private Vector2 frontAttackColliderPos;
    private bool jumpPressed = false, attackPressed = false, airAttackPressed = false, isGrounded, isSprinting = false, blockHeld = false, forceHeld = false, isUsingForce = false, isBackFlipping = false, controlIsLocked = false, deathHandled = false;
    [System.NonSerialized] public bool isDead = false;
    [System.NonSerialized] public int currentDamage, currentHealth, currentForce;
    public bool canParry { get; private set; }
    public bool isBlocking { get; private set; }
    public int damage = 3;
    Rigidbody2D rb;
    Animator an;
    GameManager gameManager;
    ForceScript forceScript;
    ContactFilter2D groundFilter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        an = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();
        forceScript = FindAnyObjectByType<ForceScript>();

        frontAttackColliderPos = attackHitBox.transform.localPosition;

        groundFilter.SetNormalAngle(80f, 100f);

        currentDamage = damage;
        currentHealth = maxHealth;
        currentForce = maxForce;

        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        forceMeter.maxValue = maxForce;
        forceMeter.value = currentForce;

        attackHitBox.SetActive(false);
        forceHitBox.SetActive(false);
    }

    void Update()
    {
        float moveX = SimpleInput.GetAxisRaw("Horizontal");
        float moveY = SimpleInput.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.J)) Attack();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();

        if (isDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (isGrounded && !deathHandled)
            {
                deathHandled = true;
                an.SetTrigger("IsDead");
                Invoke("GameOverScreen", 2f);
            }

            return;
        }

        if (controlIsLocked)
            return;

        bool wasBlocking = isBlocking;
        isBlocking = isGrounded && (blockHeld || Input.GetKey(KeyCode.K));
        an.SetBool("IsBlocking", isBlocking);

        isUsingForce = isGrounded && (forceHeld || Input.GetKey(KeyCode.I));
        an.SetBool("IsUsingForce", isUsingForce);

        if (isBlocking && !wasBlocking)
        {
            parryTimer = parryTime;
        }

        if (parryTimer > 0)
        {
            parryTimer -= Time.deltaTime;
        }

        canParry = isBlocking && parryTimer > 0;

        if (moveX != 0 && prevMoveX == 0)
        {
            float dir = Mathf.Sign(moveX);

            if (Mathf.Sign(moveX) != Mathf.Sign(transform.right.x))
            {
                lastBackTapTime = Time.time;
            }

            if (dir == lastTapDirection && Time.time - lastTapTime <= doubleTapWindow)
            {
                isSprinting = true;
            }

            lastTapTime = Time.time;
            lastTapDirection = dir;
        }

        if (moveY > 0 && prevMoveY <= 0)
        {
            lastUpTapTime = Time.time;
        }

        if (moveX == 0 || Mathf.Sign(moveX) != lastTapDirection)
        {
            isSprinting = false;
        }

        var currentState = an.GetCurrentAnimatorStateInfo(0);
        bool inBackAttack = currentState.IsName("PlayerBackAttack");
        bool inUpAttack = currentState.IsName("PlayerUpAttack");
        bool inForceAction = currentState.IsName("PlayerForcePush") || currentState.IsName("PlayerForcePull")
            || currentState.IsName("PlayerForceLightning");

        if (!inBackAttack && !isUsingForce && !isBackFlipping && !inForceAction)
        {
            if (moveX < 0)
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (moveX > 0)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        // Force Stuff
        if (isUsingForce && currentForce > 0)
        {
            int cost;

            if (moveX != 0 && prevMoveX == 0)
            {
                if (!inForceAction)
                {
                    cost = 2;
                    if (currentForce < cost) return;

                    if (Mathf.Sign(moveX) == Mathf.Sign(transform.right.x))
                    {

                        an.Play("PlayerForcePush");
                        forceScript.SetForceMode(ForceScript.ForceMode.ForcePush);
                        currentForce -= cost;
                    }
                    else
                    {
                        an.Play("PlayerForcePull");
                        forceScript.SetForceMode(ForceScript.ForceMode.ForcePull);
                        currentForce -= cost;
                    }
                }
            }

            if (moveY != 0 && prevMoveY == 0)
            {
                if (!inForceAction)
                {
                    if (Mathf.Sign(moveY) == Mathf.Sign(transform.up.y))
                    {
                        cost = 4;
                        if (currentForce < cost) return;
                        an.Play("PlayerForceLightning");
                        forceScript.SetForceMode(ForceScript.ForceMode.ForceLightning);
                        currentForce -= cost;
                    }
                }
            }

            forceMeter.value = currentForce;
            lastBackTapTime = -1f;
        }

        prevMoveX = moveX;
        prevMoveY = moveY;

        an.SetBool("IsWalking", moveX != 0);
        an.SetBool("IsSprinting", isSprinting);

        movement = new Vector2(moveX, 0f);

        // Attack Stuff
        if (attackPressed)
        {
            if (isGrounded && !isBlocking && !isUsingForce && !isBackFlipping && !inForceAction)
            {
                if (!inBackAttack && !inUpAttack)
                {
                    bool upAttack = moveY > 0 || Time.time - lastUpTapTime <= upAttackWindow;
                    bool backAttack = Time.time - lastBackTapTime <= backAttackWindow;

                    currentDamage = damage;
                    attackHitBox.transform.localPosition = frontAttackColliderPos;

                    if (upAttack)
                    {
                        an.ResetTrigger("Attack");
                        an.Play("PlayerUpAttack");
                        lastUpTapTime = -1f;
                    }
                    else if (backAttack && !isUsingForce)
                    {
                        if (transform.right.x > 0)
                        {
                            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }

                        attackHitBox.transform.localPosition = backAttackColliderPos;
                        an.ResetTrigger("Attack");
                        an.Play("PlayerBackAttack");
                        lastBackTapTime = -1f;
                    }
                    else if (!an.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack3"))
                    {
                        an.SetTrigger("Attack");
                    }
                }
            }
            else if (!isGrounded)
            {
                airAttackPressed = true;
            }

            attackPressed = false;
        }

        if (transform.position.y < -20f)
        {
            Died();
            Invoke("GameOverScreen", 1f);
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = rb.IsTouching(groundFilter);
        an.SetBool("IsGrounded", isGrounded);

        if (isGrounded && !wasGrounded)
        {
            an.SetBool("IsJumping", false);
            isBackFlipping = false;

            if (airAttackPressed)
            {
                attackHitBox.transform.localPosition = frontAttackColliderPos;
                an.Play("PlayerFallAttack");
                currentDamage += 1;
                airAttackPressed = false;
            }
        }

        if (controlIsLocked)
        {
            rb.linearVelocity = new Vector2(autoRunDirection * moveSpeed * sprintMulitplier, rb.linearVelocity.y);
            return;
        }

        if (isDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        var state = an.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("PlayerAttack1") || state.IsName("PlayerAttack2") || state.IsName("PlayerAttack3") || state.IsName("PlayerFallAttack")
            || state.IsName("PlayerBackAttack") || state.IsName("PlayerUpAttack")
            || state.IsName("PlayerForcePush") || state.IsName("PlayerForcePull") || state.IsName("PlayerForceLightning")
            || isBlocking || isUsingForce)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (isSprinting)
        {
            speed = moveSpeed * sprintMulitplier;
        }
        else
        {
            speed = moveSpeed;
        }

        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpPressed = false;
        }
    }

    public void Jump()
    {
        if (!isGrounded || isBlocking || isUsingForce || isDead || controlIsLocked)
            return;

        if (Time.time - lastBackTapTime <= backFlipWindow)
        {
            if (transform.right.x > 0)
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }

            isBackFlipping = true;
            lastBackTapTime = -1f;
            an.Play("PlayerBackFlip");
        }

        jumpPressed = true;
        an.SetBool("IsJumping", true);
    }

    public void LockControls()
    {
        controlIsLocked = true;
        autoRunDirection = 0f;
        movement = Vector2.zero;
        isSprinting = false;
        isBlocking = false;
        isUsingForce = false;

        an.SetBool("IsWalking", false);
        an.SetBool("IsSprinting", false);
        an.SetBool("IsBlocking", false);
        an.SetBool("IsUsingForce", false);
        
        attackHitBox.SetActive(false);
        forceHitBox.SetActive(false);
    }

    public void PlayerVictory()
    {
        an.Play("PlayerVictory");
    }

    public void AutoRun(float direction)
    {
        autoRunDirection = direction;

        if (direction < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        an.SetBool("IsWalking", true);
        an.SetBool("IsSprinting", true);
        an.Play("PlayerSprint");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool inFront = Mathf.Sign(collision.transform.position.x - transform.position.x) == Mathf.Sign(transform.right.x);

        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            if (inFront && isBlocking)
                return;

            TakeDamage(1);
        }

        if (collision.CompareTag("Health"))
        {
            if (currentHealth == maxHealth)
                return;

            currentHealth += 3;

            if (currentHealth >= maxHealth)
                currentHealth = maxHealth;

            healthBar.value = currentHealth;

            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Force"))
        {
            if (currentForce == maxForce)
                return;

            currentForce += 3;

            if (currentForce >= maxForce)
                currentForce = maxForce;

            forceMeter.value = currentForce;

            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead || controlIsLocked)
            return;

        currentHealth -= amount;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Died();
        }
    }

    private void Died()
    {
        isDead = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        an.SetBool("IsWalking", false);
        an.SetBool("IsSprinting", false);
        an.SetBool("IsBlocking", false);
        an.SetBool("IsUsingForce", false);

        Debug.Log("Player Dead");
    }

    private void GameOverScreen()
    {
        gameManager.GameOver();
    }

    public void Attack()
    {
        attackPressed = true;
    }

    public void EnableAttackCollider()
    {
        attackHitBox.SetActive(true);
    }

    public void DisableAttackCollider()
    {
        attackHitBox.SetActive(false);
    }

    public void EnableForceCollider()
    {
        forceHitBox.SetActive(true);
    }

    public void DisableForceCollider()
    {
        forceHitBox.SetActive(false);
    }

    public void StartBlock()
    {
        blockHeld = true;
    }

    public void StopBlock()
    {
        blockHeld = false;
    }

    public void StartForce()
    {
        forceHeld = true;
    }

    public void StopForce()
    {
        forceHeld = false;
    }
}