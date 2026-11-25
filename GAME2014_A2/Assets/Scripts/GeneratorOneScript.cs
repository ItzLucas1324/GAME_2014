using UnityEngine;

public class GeneratorOneScript : MonoBehaviour
{
    [SerializeField] GameObject exitDoor, explosion;
    [SerializeField] private int generatorHealth = 4;
    GameObject explosionInst;
    GameManager gameManager;
    Animator an;
    [System.NonSerialized] public bool generatorDestroyed = false;

    private void Start()
    {
        an = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (generatorDestroyed)
            return;

        if (collision.CompareTag("PlayerAttack"))
        {
            generatorHealth--;

            if (generatorHealth <= 0)
            {
                generatorDestroyed = true;
                gameManager.exitUnlocked = generatorDestroyed;

                explosionInst = Instantiate(explosion, transform.position, Quaternion.identity);
                Animator expAnimator = explosionInst.GetComponent<Animator>();
                float expLength = expAnimator.runtimeAnimatorController.animationClips[0].length;
                Destroy(explosionInst, expLength);
                
                an.SetBool("IsDestroyed", generatorDestroyed);
            }
        }
    }

    public void OpenDoor()
    {
        BoxCollider2D doorCollider = exitDoor.GetComponent<BoxCollider2D>();
        doorCollider.enabled = false;
        FindAnyObjectByType<GameManager>().UnlockExit();
    }
}
