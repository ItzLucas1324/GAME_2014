using UnityEngine;

public class InstructionsPlayerScript : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5.0f, doubleTapWindow = 0.3f, backAttackWindow = 0.25f, upAttackWindow = 0.25f, backFlipWindow = 0.25f;
    private float lastTapTime = -1f, lastTapDirection = 1f, lastBackTapTime = -1f, prevMoveX = 0f, lastUpTapTime = -1f, prevMoveY = 0f;
    private bool jumpPressed = false, attackPressed = false, airAttackPressed = false, isGrounded, isSprinting = false,
        isBlocking = false, blockHeld = false, forceHeld = false, isUsingForce = false, isBackFlipping = false;
    Rigidbody2D rb;
    Animator an;
    ContactFilter2D groundFilter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        an = GetComponent<Animator>();
        groundFilter.SetNormalAngle(80f, 100f);
    }

    void Update()
    {
        float moveX = SimpleInput.GetAxisRaw("Horizontal");
        float moveY = SimpleInput.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.J)) Attack();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();

        isBlocking = isGrounded && (blockHeld || Input.GetKey(KeyCode.K));
        an.SetBool("IsBlocking", isBlocking);

        isUsingForce = isGrounded && (forceHeld || Input.GetKey(KeyCode.I));
        an.SetBool("IsUsingForce", isUsingForce);

        if (moveX != 0 && prevMoveX == 0)
        {
            float dir = Mathf.Sign(moveX);

            if (dir != Mathf.Sign(transform.right.x))
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

        if (isUsingForce)
        {
            if (!inForceAction)
            {
                if (moveX != 0 && prevMoveX == 0)
                {
                    if (Mathf.Sign(moveX) == Mathf.Sign(transform.right.x))
                    {
                        an.Play("PlayerForcePush");
                    }
                    else
                    {
                        an.Play("PlayerForcePull");
                    }
                }
                else if (moveY > 0 && prevMoveY <= 0)
                {
                    an.Play("PlayerForceLightning");
                }
            }

            lastBackTapTime = -1f;
        }

        prevMoveX = moveX;
        prevMoveY = moveY;

        an.SetBool("IsWalking", moveX != 0);
        an.SetBool("IsSprinting", isSprinting);

        // Attacks
        if (attackPressed)
        {
            if (isGrounded && !isBlocking && !isUsingForce && !isBackFlipping && !inForceAction)
            {
                if (!inBackAttack && !inUpAttack)
                {
                    bool upAttack = moveY > 0 || Time.time - lastUpTapTime <= upAttackWindow;
                    bool backAttack = Time.time - lastBackTapTime <= backAttackWindow;

                    if (upAttack)
                    {
                        an.ResetTrigger("Attack");
                        an.Play("PlayerUpAttack");
                        lastUpTapTime = -1f;
                    }
                    else if (backAttack)
                    {
                        if (transform.right.x > 0)
                        {
                            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }

                        an.ResetTrigger("Attack");
                        an.Play("PlayerBackAttack");
                        lastBackTapTime = -1f;
                    }
                    else if (!currentState.IsName("PlayerAttack3"))
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
                an.Play("PlayerFallAttack");
                airAttackPressed = false;
            }
        }

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(0f, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpPressed = false;
        }
    }

    public void Jump()
    {
        if (!isGrounded || isBlocking || isUsingForce)
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

    public void Attack()
    {
        attackPressed = true;
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

    public void EnableAttackCollider() { }
    public void DisableAttackCollider() { }
    public void EnableForceCollider() { }
    public void DisableForceCollider() { }
}