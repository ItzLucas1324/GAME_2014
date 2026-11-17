using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f, jumpForce = 5.0f, sprintMulitplier = 1.8f, doubleTapWindow = 0.3f;
    private float lastRightTapTime = -1f, prevMoveX = 0f, speed;
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

        if (moveX > 0 && prevMoveX <= 0)
        {
            if (Time.time - lastRightTapTime <= doubleTapWindow)
            {
                isSprinting = true;
            }

            lastRightTapTime = Time.time;
        }

        if (moveX <= 0)
        {
            isSprinting = false;
        }

        prevMoveX = moveX;

        an.SetBool("IsWalking", moveX != 0);

        an.SetBool("IsSprinting", isSprinting);

        if (moveX != 0)
        {
            an.SetFloat("WalkDirection", Mathf.Sign(moveX));
        }

        movement = new Vector2(moveX, 0f);

        if (attackPressed)
        {
            if (isGrounded)
            {
                if (!an.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack2"))
                    an.SetTrigger("Attack");
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
        if (state.IsName("PlayerAttack") || state.IsName("PlayerAttack2"))
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
