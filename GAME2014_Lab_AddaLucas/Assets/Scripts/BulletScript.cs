using UnityEngine;

public class BulletScript : MonoBehaviour
{
    ObjectPool pool;
    Camera cam;
    GameManager manager;
    [SerializeField] private BulletType.Bullet bulletType;
    public BulletType.Bullet Type => bulletType;
    float screenBottom, screenTop;

    void Start()
    {       
        pool = FindAnyObjectByType<ObjectPool>();
        manager = FindAnyObjectByType<GameManager>();
        cam = Camera.main;
        screenBottom = cam.transform.position.y - cam.orthographicSize;
        screenTop = cam.transform.position.y + cam.orthographicSize;
    }

    void Update()
    {
        if (transform.position.y <= screenBottom || transform.position.y >= screenTop)
        {
            DeactivateBullet(gameObject);
            manager.UpdateScore(1);
        }
    }

    private void DeactivateBullet(GameObject bullet)
    {
        pool.ReturnObject(pool.enemyBulletPool, bullet);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && bulletType == BulletType.Bullet.Enemy)
        {
            DeactivateBullet(gameObject);
        }
        else if (collision.CompareTag("Enemy") && bulletType == BulletType.Bullet.Player)
        {
            DeactivateBullet(gameObject);
        }
    }
}
