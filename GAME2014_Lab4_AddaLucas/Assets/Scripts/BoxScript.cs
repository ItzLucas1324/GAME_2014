using UnityEngine;

public class BoxScript : MonoBehaviour
{
    private Vector3 spawnPosition;
    Rigidbody2D rb;
    void Start()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.y < -10.0f)
        {
            transform.position = spawnPosition;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
