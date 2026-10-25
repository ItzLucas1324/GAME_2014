using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider masterSlider, sfxSlider, musicSlider;

    private void OnEnable()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("Can't find the SoundManager!");
            return;
        }

        masterSlider.SetValueWithoutNotify(SoundManager.Instance.MasterVolume);
        sfxSlider.SetValueWithoutNotify(SoundManager.Instance.SfxVolume);
        musicSlider.SetValueWithoutNotify(SoundManager.Instance.MusicVolume);

        masterSlider.onValueChanged.AddListener(SoundManager.Instance.SetVolumeMaster);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetVolumeSFX);
        musicSlider.onValueChanged.AddListener(SoundManager.Instance.SetVolumeMusic);
    }

    private void OnDisable()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
    }
}
