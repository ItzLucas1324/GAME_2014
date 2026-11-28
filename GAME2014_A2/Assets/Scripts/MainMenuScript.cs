using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] GameObject howToPlayParent, instructionsPlayer, instructionsPlatform, healthPickup, forceMeterPickup;
    [SerializeField] GameObject[] helpScreens;
    [SerializeField] Button previousButton, nextButton;
    private int currentHelpScreen = 0;

    void Start()
    {
        instructionsPlayer.SetActive(false);
        instructionsPlatform.SetActive(false);
    }

    public void OpenLevelOne()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OpenLevelTwo()
    {
        SceneManager.LoadScene("Level2");
    }

    public void CloseHowToPlay()
    {
        howToPlayParent.SetActive(false);
        instructionsPlayer.SetActive(false);
        instructionsPlatform.SetActive(false);
        healthPickup.SetActive(false);
        forceMeterPickup.SetActive(false);
    }

    public void HowToPlayButtonPressed()
    {
        currentHelpScreen = 0;
        ShowCurrentScreen();
        instructionsPlayer.SetActive(true);
        instructionsPlatform.SetActive(true);
    }

    public void NextButtonPressed()
    {
        if (currentHelpScreen < helpScreens.Length - 1)
        {
            currentHelpScreen++;
            ShowCurrentScreen();
        }
    }

    public void PreviousButtonPressed()
    {
        if (currentHelpScreen > 0)
        {
            currentHelpScreen--;
            ShowCurrentScreen();
        }
    }

    private void ShowCurrentScreen()
    {
        for (int i = 0; i < helpScreens.Length; i++)
        {
            helpScreens[i].SetActive(i == currentHelpScreen);
        }

        previousButton.interactable = currentHelpScreen > 0;
        nextButton.interactable = currentHelpScreen < helpScreens.Length - 1;

        bool isLastScreen = currentHelpScreen == helpScreens.Length - 1;
        instructionsPlayer.SetActive(!isLastScreen);
        instructionsPlatform.SetActive(!isLastScreen);
        healthPickup.SetActive(isLastScreen);
        forceMeterPickup.SetActive(isLastScreen);
    }
}
