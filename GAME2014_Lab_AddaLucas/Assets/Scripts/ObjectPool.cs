using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject enemyBulletPrefab, playerBulletPrefab, enemyPrefab;
    public Queue<GameObject> enemyBulletPool = new Queue<GameObject>();
    public Queue<GameObject> playerBulletPool = new Queue<GameObject>();
    public Queue<GameObject> enemyPool = new Queue<GameObject>();

    BulletFactory factory = null;

    private void Start()
    {
        factory = GetComponent<BulletFactory>();
    }

    public GameObject GetBullet(GameObject bullet)
    {
        if (bullet.TryGetComponent(out BulletScript script) && script.Type == BulletType.Bullet.Enemy)
        {
            return GetObject(enemyBulletPool, enemyBulletPrefab);
        }

        return GetObject(playerBulletPool, playerBulletPrefab);
    }

    public GameObject GetEnemy()
    {
        return GetObject(enemyPool, enemyPrefab);
    }

    public void ReturnBullet(GameObject obj)
    {
        if (obj.TryGetComponent(out BulletScript script) && script.Type == BulletType.Bullet.Enemy)
        {
            GetObject(enemyBulletPool, obj);
        }
        else
        {
            GetObject(playerBulletPool, obj);
        }
    }

    public void ReturnEnemy(GameObject obj)
    {
        ReturnObject(enemyPool, obj);
    }

    public GameObject GetObject(Queue<GameObject> queue, GameObject prefab)
    {
        if (queue.Count > 0)
        {
            GameObject obj = enemyBulletPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        
        return Instantiate(prefab);
        
        //GameObject tempBullet = factory.CreateBullet();
    }

    public void ReturnObject(Queue<GameObject> queue, GameObject obj)
    {
        obj.SetActive(false);
        queue.Enqueue(obj);
    }
}
