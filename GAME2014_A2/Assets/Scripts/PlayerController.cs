using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f, jumpForce = 5.0f;
    private Vector2 movement;
    private bool jumpPressed = false, isGrounded;
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

        if (moveX != 0)
        {
            an.SetBool("IsWalking", true);
        }
        else
        {
            an.SetBool("IsWalking", false);
        }

        movement = new Vector2(moveX, 0f);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && collision.GetContact(0).normal.y > 0.8f)
        {
            an.SetBool("IsJumping", false);
            an.SetBool("IsGrounded", true);
            isGrounded = true;
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
