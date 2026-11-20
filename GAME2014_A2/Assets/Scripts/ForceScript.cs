using UnityEngine;

public class ForceScript : MonoBehaviour
{
    BoxCollider2D boxCol;
    [SerializeField] private Vector2 forceLightningSize, forceLightningOffset;
    Vector2 regularSize, regularOffset;
    public ForceMode mode { get; private set; }
    public int damage { get; private set; }

    public enum ForceMode
    {
        None,
        ForcePush,
        ForcePull,
        ForceLightning
    }

    void Start()
    {
        mode = ForceMode.None;

        boxCol = GetComponent<BoxCollider2D>();
        regularSize = new Vector2(boxCol.size.x, boxCol.size.y);
        regularOffset = new Vector2(boxCol.offset.x, boxCol.offset.y);
    }

    void Update()
    {
        
    }

    public void SetForceMode(ForceMode forceMode)
    {
        mode = forceMode;

        switch (mode)
        {
            case ForceMode.ForcePush:

                boxCol.size = regularSize;
                boxCol.offset = regularOffset;
                SetDamage(2);

                break;
            case ForceMode.ForcePull:

                boxCol.size = regularSize;
                boxCol.offset = regularOffset;
                SetDamage(2);

                break;
            case ForceMode.ForceLightning:

                boxCol.size = forceLightningSize;
                boxCol.offset = forceLightningOffset;
                SetDamage(3);

                break;
            default:
                mode = ForceMode.None;
                break;
        }
    }

    private void SetDamage(int amount)
    {
        damage = amount;
    }
}
