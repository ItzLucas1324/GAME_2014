using UnityEngine;

public class BulletFactory : MonoBehaviour
{
    [SerializeField] private GameObject enemyBullet;
    [SerializeField] private GameObject playerBullet;

    public GameObject CreateBullet(BulletType.Bullet type)
    {
        GameObject tempBullet = null;

        switch (type)
        {
            case BulletType.Bullet.Enemy:
                tempBullet = Instantiate(enemyBullet);
                break;
            case BulletType.Bullet.Player:
                tempBullet = Instantiate(playerBullet);
                break;
        }

        tempBullet.transform.SetParent(transform);
        tempBullet.SetActive(false);
        return tempBullet;
    }
}
