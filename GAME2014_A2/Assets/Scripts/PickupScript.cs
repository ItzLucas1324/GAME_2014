using UnityEngine;

public class PickupScript : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.5f, bobSpeed = 1f;
    Vector2 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newYPos = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector2(transform.position.x, newYPos);
    }
}
