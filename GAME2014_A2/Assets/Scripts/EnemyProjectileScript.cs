using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    [SerializeField] private float speed = 8.0f, lifetime = 5f;
    [SerializeField] private int damage = 1;
    private float direction;
    private bool isDeflected = false;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(float dir)
    {
        direction = dir;
        SetMovement();
        Destroy(gameObject, lifetime);
    }

    private void SetMovement()
    {
        rb.linearVelocity = new Vector2(direction * speed, 0f);

        if (direction < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
            return;
        }

        if (!isDeflected)
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                bool inFront = Mathf.Sign(transform.position.x - player.transform.position.x) == Mathf.Sign(player.transform.right.x);

                if (inFront && player.canParry)
                {
                    direction = -direction;
                    isDeflected = true;
                    gameObject.tag = "Untagged";
                    SetMovement();
                }
                else if (inFront && player.isBlocking)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            EnemyAI enemy = collision.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);            }
        }
    }
}
