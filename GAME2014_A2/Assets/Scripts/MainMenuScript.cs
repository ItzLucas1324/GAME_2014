using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    void Start()
    {
        
    }

    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
