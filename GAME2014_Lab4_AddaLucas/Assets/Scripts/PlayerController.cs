using Terresquall;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f, jumpforce = 5.0f;
    private int maxJumps = 2, jumpCounter = 0;
    private Vector2 movement;
    private bool jumpPressed = false;
    Rigidbody2D rb;
    Animator an;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        an = GetComponent<Animator>();
    }

    
    void Update()
    {
        float moveX = VirtualJoystick.GetAxisRaw("Horizontal", 1);
        movement = new Vector2(moveX, 0.0f);

        if (moveX != 0)
        {
            an.SetBool("IsWalking", true);

            if (moveX < 0)
            {
                transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }
            else if (moveX > 0)
            {
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
        }
        else if (moveX == 0)
        {
            an.SetBool("IsWalking", false);
        }
    }

    public void Jump()
    {
        if (jumpCounter < maxJumps)
        {
            jumpCounter++;
            jumpPressed = true;
            an.SetBool("IsJumping", true);
            an.Play("PlayerJump", 0, 0f);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
            jumpPressed = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCounter = 0;
            an.SetBool("IsJumping", false);
        }
    }
}
