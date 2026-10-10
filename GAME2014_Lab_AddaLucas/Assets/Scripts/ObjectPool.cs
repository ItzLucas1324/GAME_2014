using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject bulletPrefab, enemyPrefab;
    public Queue<GameObject> bulletPool = new Queue<GameObject>();

    public Queue<GameObject> enemyPool = new Queue<GameObject>();

    public GameObject GetBullet()
    {
        return GetObject(bulletPool, bulletPrefab);
    }

    public GameObject GetEnemy()
    {
        return GetObject(enemyPool, enemyPrefab);
    }

    public void ReturnBullet(GameObject obj)
    {
        ReturnObject(bulletPool, obj);
    }

    public void ReturnEnemy(GameObject obj)
    {
        ReturnObject(enemyPool, obj);
    }

    public GameObject GetObject(Queue<GameObject> queue, GameObject prefab)
    {
        if (queue.Count > 0)
        {
            GameObject obj = bulletPool.Dequeue();
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
