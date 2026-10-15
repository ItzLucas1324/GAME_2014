using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private Boundary horizontalBounds;
    [SerializeField] private Boundary speedRange;
    [SerializeField] private Transform bulletSpawn;
    private ObjectPool pool;
    private float speed, timeOffset, halfWidth, shootTime;
    private bool needsInit = true;
    public float bulletSpeed = 10f, shotTimeMin = 1.0f, shotTimeMax = 2.0f;
    private Camera cam;

    void Start()
    {
        pool = FindAnyObjectByType<ObjectPool>();

        speed = Random.Range(speedRange.min, speedRange.max);
        timeOffset = Random.Range(0f, 10f);

        cam = Camera.main;
        halfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;

        shootTime = Random.Range(shotTimeMin, shotTimeMax);
    }

    private void OnEnable()
    {
        needsInit = true;
    }

    private void Initialize(float min, float max)
    {
        speed = Random.Range(speedRange.min, speedRange.max);
        shootTime = Random.Range(shotTimeMin, shotTimeMax);

        float startX = Mathf.Clamp(transform.position.x, min, max);
        timeOffset = (startX - min) / speed - Time.time;

        needsInit = false;
    }

    void Update()
    {
        float min = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x + halfWidth;
        float max = cam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x - halfWidth;

        if (needsInit)
            Initialize(min, max);

        shootTime -= Time.deltaTime;

        float x = min + Mathf.PingPong((Time.time + timeOffset) * speed, max - min);
        transform.position = new Vector2(x, transform.position.y);

        if (shootTime <= 0)
            Shoot();
    }

    private void Shoot()
    {
        GameObject bullet = pool.GetBullet(pool.enemyBulletPrefab);
        bullet.transform.position = bulletSpawn.transform.position;
        bullet.transform.rotation = bulletSpawn.transform.rotation;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = -transform.up * bulletSpeed;

        shootTime = Random.Range(shotTimeMin, shotTimeMax);
    }
}
