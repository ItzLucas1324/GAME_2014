using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.0f, -0.2f);

    RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        Rect rect = rawImage.uvRect;
        rect.position += scrollSpeed * Time.deltaTime;
        rawImage.uvRect = rect;
    }
}
