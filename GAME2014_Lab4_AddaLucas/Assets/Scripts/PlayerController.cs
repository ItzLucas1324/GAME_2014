using Terresquall;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    private Vector2 movement;
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

    private void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed * Time.deltaTime;   
    }
}
