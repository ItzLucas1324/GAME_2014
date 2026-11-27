using UnityEngine;

public class CreditScript : MonoBehaviour
{
    Animator an;
    PickupScript pickup;
    PlayerController player;
    CircleCollider2D cirCollider;
    private Quaternion collectRotation;
    private Vector3 followOffset;
    private bool collected = false;
    void Start()
    {
        an = GetComponentInChildren<Animator>();
        pickup = GetComponent<PickupScript>();
        cirCollider = GetComponent<CircleCollider2D>();
        player = FindAnyObjectByType<PlayerController>();
    }

    private void LateUpdate()
    {
        if (!collected)
            return;

        transform.position = player.transform.position + followOffset;
        transform.rotation = collectRotation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickup.canBump = false;

            float facing = Mathf.Sign(collision.transform.right.x);

            if (facing < 0)
            {
                collectRotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else
            {
                collectRotation = Quaternion.identity;
            }

            followOffset = transform.position - player.transform.position;
            cirCollider.enabled = false;
            collected = true;

            player.SetScore(1);

            an.Play("CoinCollected");

            Destroy(gameObject, 0.8f);
        }
    }
}
