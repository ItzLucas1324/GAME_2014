using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI, helpMenuUI;
    private bool isHelpMenuOpen = false;

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void HelpMenu()
    {
        isHelpMenuOpen = !isHelpMenuOpen;

        mainMenuUI.SetActive(isHelpMenuOpen);
        helpMenuUI.SetActive(!isHelpMenuOpen);
    }
}
