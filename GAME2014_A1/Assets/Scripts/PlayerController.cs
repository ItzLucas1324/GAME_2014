using UnityEngine;
using Terresquall;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform topBound, bottomBound;
    [SerializeField] private Animator an;
    [SerializeField] private float turnSpeed = 4.0f;
    Vector2 movement;
    public float moveSpeed = 2.0f;
    Rigidbody2D rb;
    private Camera cam;
    private float halfWidth, turn;

    private void Awake()
    {
        cam = Camera.main;
        halfWidth = GetComponent<Collider2D>().bounds.extents.x;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = VirtualJoystick.GetAxis("Horizontal", 1);
        float moveY = VirtualJoystick.GetAxis("Vertical", 1);

        movement = new Vector2(moveX, moveY).normalized;

        CheckBounds();

        turn = Mathf.MoveTowards(turn, moveX, turnSpeed * Time.deltaTime);
        an.SetFloat("BankTime", (turn + 1.0f) / 2.0f);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    private void CheckBounds()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, bottomBound.position.y, topBound.position.y);
        transform.position = pos;

        float left = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        float right = cam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;

        pos.x = Mathf.Clamp(pos.x, left + halfWidth, right - halfWidth);
        transform.position = pos;
    }
}
