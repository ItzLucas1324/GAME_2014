using UnityEngine;

public class BulletScript : MonoBehaviour
{
    ObjectPool pool;
    Camera cam;
    GameManager manager;
    PlayerBehaviour player;
    EnemySpawn enemySpawn;
    [SerializeField] private BulletType.Bullet bulletType;
    public BulletType.Bullet Type => bulletType;
    float screenBottom, screenTop;

    void Start()
    {       
        pool = FindAnyObjectByType<ObjectPool>();
        manager = FindAnyObjectByType<GameManager>();
        player = FindAnyObjectByType<PlayerBehaviour>();
        cam = Camera.main;
        screenBottom = cam.transform.position.y - cam.orthographicSize;
        screenTop = cam.transform.position.y + cam.orthographicSize;
    }

    void Update()
    {
        if (transform.position.y <= screenBottom && bulletType == BulletType.Bullet.Enemy)
        {
            DeactivateBullet();
        }
        else if (transform.position.y >= screenTop && bulletType == BulletType.Bullet.Player)
        {
            DeactivateBullet();
        }
    }

    private void DeactivateBullet()
    {
        if (bulletType == BulletType.Bullet.Enemy)
        {
            pool.ReturnObject(pool.enemyBulletPool, gameObject);
        }
        else if (bulletType == BulletType.Bullet.Player)
        {
            pool.ReturnObject(pool.playerBulletPool, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && bulletType == BulletType.Bullet.Enemy)
        {
            DeactivateBullet();
            player.DealPlayerHealth(2);
        }
        else if (collision.CompareTag("Enemy") && bulletType == BulletType.Bullet.Player)
        {
            DeactivateBullet();
            manager.UpdateScore(2);
            Destroy(collision.gameObject);

            enemySpawn = FindAnyObjectByType<EnemySpawn>();
            enemySpawn.OnEnemyKilled();
        }
    }
}
