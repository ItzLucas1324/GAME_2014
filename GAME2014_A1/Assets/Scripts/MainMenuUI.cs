using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI, helpMenuUI, settingsMenuUI;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Slider masterSlider, sfxSlider, musicSlider;
    private bool isHelpMenuOpen = false, isSettingsMenuOpen = false;

    private void Start()
    {
        UpdateHighScoreText();

        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("Can't find SoundManager!");
            return;
        }

        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("Can't find SoundManager!");
            return;
        }

        SoundManager.Instance.PlayMusicWithIntro("unchained_destiny_opening", "unchained_destiny_loop");

        masterSlider.SetValueWithoutNotify(SoundManager.Instance.MasterVolume);
        sfxSlider.SetValueWithoutNotify(SoundManager.Instance.SfxVolume);
        musicSlider.SetValueWithoutNotify(SoundManager.Instance.MusicVolume);

        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    private void UpdateHighScoreText()
    {
        if (HighScoreTracker.Instance != null)
        {
            highScoreText.text = "High Score\nis\n" + HighScoreTracker.Instance.HighScore + "!";
        }
        else
        {
            Debug.LogWarning("Can't find HighScoreTracker");
        }
    }

    public void HelpMenu()
    {
        isHelpMenuOpen = !isHelpMenuOpen;

        mainMenuUI.SetActive(!isHelpMenuOpen);
        helpMenuUI.SetActive(isHelpMenuOpen);
    }

    public void SettingsMenu()
    {
        isSettingsMenuOpen = !isSettingsMenuOpen;

        mainMenuUI.SetActive(!isSettingsMenuOpen);
        settingsMenuUI.SetActive(isSettingsMenuOpen);
    }

    public void ResetHighScore()
    {
        if (HighScoreTracker.Instance != null)
        {
            HighScoreTracker.Instance.ResetHighScore();
            UpdateHighScoreText();
        }
    }

    public void OnMasterChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetVolumeMaster(value);
        }
    }

    public void OnSfxChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetVolumeSFX(value);
        }
    }

    public void OnMusicChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetVolumeMusic(value);
        }
    }
}
