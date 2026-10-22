using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private ObjectManager.ObjectType bulletType;
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
}
