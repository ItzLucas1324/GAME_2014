using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private Boundary horizontalBounds;
    [SerializeField] private Boundary speedRange;

    private float speed, timeOffset, halfWidth;
    private Camera cam;

    void Start()
    {
        speed = Random.Range(speedRange.min, speedRange.max);
        timeOffset = Random.Range(0f, 10f);

        cam = Camera.main;
        halfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
    }

    void Update()
    {
        float min = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x + halfWidth;
        float max = cam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x - halfWidth;

        float x = min + Mathf.PingPong((Time.time + timeOffset) * speed, max - min);
        transform.position = new Vector2(x, transform.position.y);
    }
}
