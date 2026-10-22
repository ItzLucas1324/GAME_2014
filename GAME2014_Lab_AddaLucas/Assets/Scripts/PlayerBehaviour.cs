using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private float horizontalForce, time;
    [SerializeField] private Boundary horizontalBounds;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] Slider healthSlider;
    ObjectPool pool;
    public float shootingTime = 0.5f, shootingSpeed = 8.0f;

    [SerializeField] private int health;
    private int currentHealth;

    [Range(0.0f, 1.0f)] public float decay;

    private Rigidbody2D rb;

    void Start()
    {
        health = 10;
        currentHealth = health;
        healthSlider.maxValue = health;
        healthSlider.value = currentHealth;
        pool = FindAnyObjectByType<ObjectPool>();
        rb = GetComponent<Rigidbody2D>();
        time = shootingTime;
    }

    void FixedUpdate()
    {
        rb.linearVelocity *= (1.0f * decay);
        Move();
        CheckBounds();
    }

    private void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject bullet = pool.GetBullet(BulletType.Bullet.Player);
        bullet.transform.position = bulletSpawn.transform.position;
        bullet.transform.rotation = bulletSpawn.transform.rotation;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = transform.up * shootingSpeed;

        time = shootingTime;
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

    public void DealPlayerHealth(int amount)
    {
        currentHealth -= amount;
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            SceneManager.LoadScene("End");
        }
    }
}
