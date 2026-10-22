using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public ObjectManager.ObjectType bulletType;
    [SerializeField] private float offscreenMargin = 1f;
    private ObjectManager manager;
    private float bottomBound;
    private float topBound;

    private void Awake()
    {
        manager = FindAnyObjectByType<ObjectManager>();

        Camera cam = Camera.main;
        bottomBound = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y - offscreenMargin;
        topBound = cam.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y + offscreenMargin;
    }

    private void Update()
    {
        float y = transform.position.y;

        if (bulletType == ObjectManager.ObjectType.EnemyBullet)
        {
            if (y < bottomBound)
            {
                manager.ReturnObject(bulletType, gameObject);
            }
        }
        else
        {
            if (y > topBound)
            {
                manager.ReturnObject(bulletType, gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && bulletType == ObjectManager.ObjectType.PlayerBullet)
        {
            GameObject enemy = collision.gameObject;
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyController.TakeDamage(1);

            manager.ReturnObject(ObjectManager.ObjectType.PlayerBullet, gameObject);
        }

        if (collision.CompareTag("Player") && bulletType == ObjectManager.ObjectType.EnemyBullet)
        {
            GameObject player = collision.gameObject;
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.TakeDamage(1);

            manager.ReturnObject(ObjectManager.ObjectType.EnemyBullet, gameObject);
        }
    }
}
