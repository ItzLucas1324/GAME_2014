using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform enemySpawn;
    [SerializeField] private float offscreenMargin = 1f, spawnTimer = 3.0f;
    [SerializeField] private int amountOfEnemysToSpawn;
    Camera cam;
    ObjectManager manager;
    [System.NonSerialized] public int currentEnemyCount;
    private float topBound, leftBound, rightBound, timer = 0f;

    void Awake()
    {
        cam = Camera.main;
        manager = GetComponent<ObjectManager>();
        topBound = cam.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y + offscreenMargin;
        leftBound = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x + offscreenMargin;
        rightBound = cam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x - offscreenMargin;
        enemySpawn.transform.position = new Vector3(0f, topBound, 0f);
        currentEnemyCount = 0;
    }

    void Update()
    {
        if (currentEnemyCount < amountOfEnemysToSpawn)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemyInst = manager.GetObject(ObjectManager.ObjectType.Enemy);
        float randomXSpawn = Random.Range(leftBound, rightBound);
        enemyInst.transform.position = new Vector3(randomXSpawn, enemySpawn.transform.position.y, 0f);

        currentEnemyCount++;
        timer = spawnTimer;
    }
}
