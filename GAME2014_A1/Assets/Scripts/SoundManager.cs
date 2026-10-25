using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private AudioClip[] musicClips;

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicDictionary = new Dictionary<string, AudioClip>();

    private AudioSource sfxSource;
    private AudioSource musicSource;
    private AudioSource musicLoopSource;

    private float sfxVolume = 1f;
    private float musicVolume = 1f;
    private float masterVolume = 1f;

    public float MasterVolume { get { return masterVolume; } }
    public float SfxVolume { get { return sfxVolume; } }
    public float MusicVolume { get { return musicVolume; } }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicLoopSource = gameObject.AddComponent<AudioSource>();

        sfxSource.playOnAwake = false;
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicLoopSource.playOnAwake = false;
        musicLoopSource.loop = true;

        foreach (AudioClip clip in sfxClips)
        {
            sfxDictionary[clip.name] = clip;
        }

        foreach (AudioClip clip in musicClips)
        {
            musicDictionary[clip.name] = clip;
        }

        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        sfxSource.volume = sfxVolume * masterVolume;
        musicSource.volume = musicVolume * masterVolume;
    }

    public void PlaySound(string soundKey)
    {
        if (sfxDictionary.TryGetValue(soundKey, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("Sound key " + soundKey + " not found in SFX Dictionary.");
        }
    }

    public void PlayMusic(string soundKey)
    {
        if (musicDictionary.TryGetValue(soundKey, out AudioClip clip))
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogError("Sound key " + soundKey + " not found in Music Dictionary.");
        }
    }

    public void PlayMusicWithIntro(string introKey, string loopKey)
    {
        if (!musicDictionary.TryGetValue(introKey, out AudioClip intro) ||
            !musicDictionary.TryGetValue(loopKey, out AudioClip loop))
        {
            Debug.LogError("Intro or loop key not found in Music Dictionary.");
            return;
        }

        StopMusic();

        double startTime = AudioSettings.dspTime + 0.1;
        double introLength = (double)intro.samples / intro.frequency;

        musicSource.clip = intro;
        musicSource.loop = false;
        musicSource.PlayScheduled(startTime);

        musicLoopSource.clip = loop;
        musicLoopSource.loop = true;
        musicLoopSource.PlayScheduled(startTime + introLength);
    }

    public void SetVolumeSFX(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume * masterVolume;
    }

    public void SetVolumeMusic(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume * masterVolume;
    }

    public void SetVolumeMaster(float volume)
    {
        masterVolume = volume;
        sfxSource.volume = sfxVolume * masterVolume;
        musicSource.volume = musicVolume * masterVolume;
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicLoopSource.Stop();
    }
}
