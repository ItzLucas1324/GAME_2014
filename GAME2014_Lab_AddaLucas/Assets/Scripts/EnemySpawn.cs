using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    ObjectPool pool;
    [SerializeField] private Transform spawnLocY;
    private float screenRight, screenLeft;
    Camera cam;

    public int numOfEnemies = 3;

    void Start()
    {
        pool = GetComponent<ObjectPool>();
        cam = Camera.main;
        float halfWidth = cam.orthographicSize * cam.aspect;
        screenRight = cam.transform.position.x + halfWidth;
        screenLeft = cam.transform.position.x - halfWidth;

        for (int i = 0; i < numOfEnemies; i++)
            SpawnEnemy();
    }

    void SpawnEnemy()
    {
        float spawnLocX = Random.Range(screenLeft, screenRight);

        GameObject enemy = pool.GetEnemy();
        enemy.transform.position = new Vector2(spawnLocX, spawnLocY.position.y);
    }
}
