using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] float speed = 0.1f;

    RawImage img;
    RectTransform rect;
    float offset;

    void Awake()
    {
        img = GetComponent<RawImage>();
        rect = (RectTransform)transform;
    }

    void Update()
    {
        offset = Mathf.Repeat(offset + speed * Time.deltaTime, 1f);

        Rect r = rect.rect;
        float tileAspect = (float)img.texture.height / img.texture.width;
        float tilesVisible = r.height / (r.width * tileAspect);

        img.uvRect = new Rect(0f, offset, 1f, tilesVisible);
    }
}
