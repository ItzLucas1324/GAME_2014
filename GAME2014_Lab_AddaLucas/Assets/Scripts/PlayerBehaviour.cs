using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private float horizontalForce;
    [SerializeField] private Boundary horizontalBounds;

    [Range(0.0f, 1.0f)] public float decay;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity *= (1.0f * decay);
        Move();
        CheckBounds();
    }

    private float GetTouchDirections()
    {
        if (Touchscreen.current == null)
        {
            return 0.0f;
        }

        var touch = Touchscreen.current.primaryTouch;

        if (!touch.press.isPressed)
        {
            return 0.0f;
        }

        Vector2 pos = touch.position.ReadValue();

        if (pos.x < Screen.width / 2)
        {
            return -1.0f;
        }

        return 1.0f;
    }

    private void Move()
    {
        float x = GetTouchDirections();
        rb.AddForce(new Vector2(x * horizontalForce, 0.0f));
    }

    private void CheckBounds()
    {
        Vector2 pos = rb.position;

        if (pos.x < horizontalBounds.min)
        {
            pos.x = horizontalBounds.min;
            rb.position = pos;
            rb.linearVelocity = new Vector2(0.0f, rb.linearVelocity.y);
        }
        else if (pos.x > horizontalBounds.max)
        {
            pos.x = horizontalBounds.max;
            rb.position = pos;
            rb.linearVelocity = new Vector2(0.0f, rb.linearVelocity.y);
        }
    }
}
