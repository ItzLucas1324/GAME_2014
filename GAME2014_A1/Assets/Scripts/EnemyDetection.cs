using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    GameObject player;
    PlayerController playerController;
    public bool canShoot = false;

    void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            playerController.timer = 0f;
            canShoot = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            canShoot = false;
        }
    }
}
