using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

public class BulletScript : MonoBehaviour
{
    ObjectPool pool;
    Camera cam;
    GameManager manager;
    float screenBottom;

    void Start()
    {
        pool = FindAnyObjectByType<ObjectPool>();
        manager = FindAnyObjectByType<GameManager>();
        cam = Camera.main;
        screenBottom = cam.transform.position.y - cam.orthographicSize;
    }

    void Update()
    {
        if (transform.position.y <= screenBottom)
        {
            DeactivateBullet(gameObject);
            manager.UpdateScore(1);
        }
    }

    private void DeactivateBullet(GameObject bullet)
    {
        pool.ReturnObject(pool.bulletPool, bullet);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DeactivateBullet(gameObject);
        }
    }
}
