using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    private Transform cam;
    private Vector3 startPos, camStart;

    void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position;
        camStart = cam.position;
    }

    void LateUpdate()
    {
        Vector3 camMoved = cam.position - camStart;
        transform.position = startPos + new Vector3(camMoved.x * (1 - speed), camMoved.y * (1 - speed), 0);
    }
}
