using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab, enemyBulletPrefab, playerBulletPrefab;
    private Queue<GameObject> enemyBulletPool = new Queue<GameObject>();
    private Queue<GameObject> playerBulletPool = new Queue<GameObject>();
    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    public enum ObjectType
    {
        PlayerBullet,
        EnemyBullet,
        Enemy
    }

    public GameObject GetObject(ObjectType type)
    {
        Queue<GameObject> pool = GetPool(type);
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = ObjectFactory(type);
        }

        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(ObjectType type, GameObject obj)
    {
        if (!obj.activeSelf)
        {
            return;
        }

        obj.SetActive(false);
        GetPool(type).Enqueue(obj);
    }

    private Queue<GameObject> GetPool(ObjectType type)
    {
        switch (type)
        {
            case ObjectType.PlayerBullet:
                return playerBulletPool;
            case ObjectType.EnemyBullet:
                return enemyBulletPool;
            case ObjectType.Enemy:
                return enemyPool;
            default:
                return null;
        }
    }

    private GameObject ObjectFactory(ObjectType objectToMake)
    {
        GameObject prefab = null;

        switch (objectToMake)
        {
            case ObjectType.EnemyBullet:
                prefab = enemyBulletPrefab;
                break;
            case ObjectType.PlayerBullet:
                prefab = playerBulletPrefab;
                break;
            case ObjectType.Enemy:
                prefab = enemyPrefab;
                break;
        }

        return Instantiate(prefab);
    }
}
