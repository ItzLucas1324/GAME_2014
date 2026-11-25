using UnityEngine;

public class GeneratorOneScript : MonoBehaviour
{
    [SerializeField] GameObject exitDoor, explosion;
    [SerializeField] private int generatorHealth = 4;
    GameObject explosionInst;
    Animator an;
    private bool generatorDestroyed = false;

    private void Start()
    {
        an = GetComponent<Animator>();
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

                explosionInst = Instantiate(explosion, transform.position, Quaternion.identity);
                Animator expAnimator = explosionInst.GetComponent<Animator>();
                float expLength = expAnimator.runtimeAnimatorController.animationClips[0].length;
                Destroy(explosionInst, expLength);
                
                an.SetBool("IsDestroyed", generatorDestroyed);
                OpenDoor();
            }

            Debug.Log("Generator Health: " + generatorHealth);
        }
    }

    private void OpenDoor()
    {
        BoxCollider2D doorCollider = exitDoor.GetComponent<BoxCollider2D>();
        doorCollider.enabled = false;
        FindAnyObjectByType<GameManager>().UnlockExit();
    }
}
