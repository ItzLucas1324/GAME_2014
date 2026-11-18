using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f, jumpForce = 5.0f, sprintMulitplier = 1.8f, doubleTapWindow = 0.3f, backAttackWindow = 0.25f, upAttackWindow = 0.25f;
    private float lastTapTime = -1f, lastTapDirection = 1f, lastBackTapTime = -1f, prevMoveX = 0f, lastUpTapTime = -1f, prevMoveY = 0f, speed;
    private Vector2 movement;
    private bool jumpPressed = false, attackPressed = false, airAttackPressed = false, isGrounded, isSprinting = false;
    Rigidbody2D rb;
    Animator an;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        an = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = SimpleInput.GetAxisRaw("Horizontal");
        float moveY = SimpleInput.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.J)) Attack();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();

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

        bool inBackAttack = an.GetCurrentAnimatorStateInfo(0).IsName("PlayerBackAttack");
        bool inUpAttack = an.GetCurrentAnimatorStateInfo(0).IsName("PlayerUpAttack");

        if (!inBackAttack)
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

        prevMoveX = moveX;
        prevMoveY = moveY;

        an.SetBool("IsWalking", moveX != 0);

        an.SetBool("IsSprinting", isSprinting);

        movement = new Vector2(moveX, 0f);

        if (attackPressed)
        {
            if (isGrounded)
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
                    else if (!an.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack2"))
                    {
                        an.SetTrigger("Attack");
                    }
                }
            }
            else
            {
                airAttackPressed = true;
            }

            attackPressed = false;
        }
    }

    private void FixedUpdate()
    {
        var state = an.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("PlayerAttack1") || state.IsName("PlayerAttack2") || state.IsName("PlayerFallAttack") 
            || state.IsName("PlayerBackAttack") || state.IsName("PlayerUpAttack"))
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

        if (jumpPressed )
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpPressed = false;
        }
    }

    public void Jump()
    {
        if (!isGrounded)
            return;

        jumpPressed = true;
        an.SetBool("IsJumping", true);
    }

    public void Attack()
    {
        attackPressed = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && collision.GetContact(0).normal.y > 0.8f)
        {
            an.SetBool("IsJumping", false);
            an.SetBool("IsGrounded", true);
            isGrounded = true;

            if (airAttackPressed)
            {
                an.Play("PlayerFallAttack");
                airAttackPressed = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            an.SetBool("IsGrounded", false);
            isGrounded = false;
        }
    }
}
