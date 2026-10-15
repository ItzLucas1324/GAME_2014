using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Queue<GameObject> enemyBulletPool = new Queue<GameObject>();
    public Queue<GameObject> playerBulletPool = new Queue<GameObject>();
    public Queue<GameObject> enemyPool = new Queue<GameObject>();

    BulletFactory factory;

    private void Awake()
    {
        factory = GetComponent<BulletFactory>();
    }

    public GameObject GetBullet(BulletType.Bullet type)
    {
        Queue<GameObject> queue = playerBulletPool;

        if (type == BulletType.Bullet.Enemy)
        {
            queue = enemyBulletPool;
        }

        GameObject obj;

        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            obj = factory.CreateBullet(type);
        }

        obj.SetActive(true);
        return obj;
    }

    public GameObject GetEnemy()
    {
        return GetObject(enemyPool, enemyPrefab);
    }

    public void ReturnBullet(GameObject obj, BulletType.Bullet type)
    {
        if (type == BulletType.Bullet.Enemy)
        {
            ReturnObject(enemyBulletPool, obj);
        }
        else
        {
            ReturnObject(playerBulletPool, obj);
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
            GameObject obj = queue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        
        return Instantiate(prefab);        
    }

    public void ReturnObject(Queue<GameObject> queue, GameObject obj)
    {
        obj.SetActive(false);
        queue.Enqueue(obj);
    }
}
