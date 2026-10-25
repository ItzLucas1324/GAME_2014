using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public class SoundEntry
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [SerializeField] private SoundEntry[] sfxClips;
    [SerializeField] private SoundEntry[] musicClips;

    private Dictionary<string, SoundEntry> sfxDictionary = new Dictionary<string, SoundEntry>();
    private Dictionary<string, SoundEntry> musicDictionary = new Dictionary<string, SoundEntry>();

    private AudioSource sfxSource;
    private AudioSource sfxPitchSource;
    private AudioSource musicSource;
    private AudioSource musicLoopSource;

    private float sfxVolume = 1f;
    private float musicVolume = 1f;
    private float masterVolume = 1f;
    private float musicTrackVolume = 1f;      
    private float musicLoopTrackVolume = 1f;  
    private string currentLoopKey;

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
        sfxPitchSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicLoopSource = gameObject.AddComponent<AudioSource>();

        sfxSource.playOnAwake = false;
        sfxPitchSource.playOnAwake = false;
        musicSource.playOnAwake = false;
        musicLoopSource.playOnAwake = false;
        musicSource.loop = true;
        musicLoopSource.loop = true;

        foreach (SoundEntry entry in sfxClips)
        {
            if (entry.clip == null) continue;
            sfxDictionary[entry.clip.name] = entry;
        }

        foreach (SoundEntry entry in musicClips)
        {
            if (entry.clip == null) continue;
            musicDictionary[entry.clip.name] = entry;
        }

        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        ApplySfxVolume();
        ApplyMusicVolume();
    }

    public void PlaySound(string soundKey, bool randomPitch = false)
    {
        if (!sfxDictionary.TryGetValue(soundKey, out SoundEntry entry))
        {
            Debug.LogError("Sound key " + soundKey + " not found in SFX Dictionary.");
            return;
        }

        if (randomPitch)
        {
            sfxPitchSource.pitch = Random.Range(0.9f, 1.1f);
            sfxPitchSource.PlayOneShot(entry.clip, entry.volume);
        }
        else
        {
            sfxSource.PlayOneShot(entry.clip, entry.volume);
        }
    }

    public void PlayMusic(string soundKey)
    {
        if (!musicDictionary.TryGetValue(soundKey, out SoundEntry entry))
        {
            Debug.LogError("Sound key " + soundKey + " not found in Music Dictionary.");
            return;
        }

        StopMusic();

        musicTrackVolume = entry.volume;
        ApplyMusicVolume();

        musicSource.clip = entry.clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayMusicWithIntro(string introKey, string loopKey)
    {
        if (currentLoopKey == loopKey && (musicSource.isPlaying || musicLoopSource.isPlaying))
        {
            return;
        }

        if (!musicDictionary.TryGetValue(introKey, out SoundEntry intro) ||
            !musicDictionary.TryGetValue(loopKey, out SoundEntry loop))
        {
            Debug.LogError("Intro or loop key not found in Music Dictionary.");
            return;
        }

        StopMusic();
        currentLoopKey = loopKey;

        musicTrackVolume = intro.volume;
        musicLoopTrackVolume = loop.volume;
        ApplyMusicVolume();

        double startTime = AudioSettings.dspTime + 0.1;
        double introLength = (double)intro.clip.samples / intro.clip.frequency;

        musicSource.clip = intro.clip;
        musicSource.loop = false;
        musicSource.PlayScheduled(startTime);

        musicLoopSource.clip = loop.clip;
        musicLoopSource.loop = true;
        musicLoopSource.PlayScheduled(startTime + introLength);
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicLoopSource.Stop();
        currentLoopKey = null;
    }

    public void SetVolumeSFX(float volume)
    {
        sfxVolume = volume;
        ApplySfxVolume();
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
    }

    public void SetVolumeMusic(float volume)
    {
        musicVolume = volume;
        ApplyMusicVolume();
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void SetVolumeMaster(float volume)
    {
        masterVolume = volume;
        ApplySfxVolume();
        ApplyMusicVolume();
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }

    private void ApplySfxVolume()
    {
        float v = sfxVolume * masterVolume;
        sfxSource.volume = v;
        sfxPitchSource.volume = v;
    }

    private void ApplyMusicVolume()
    {
        float v = musicVolume * masterVolume;
        musicSource.volume = v * musicTrackVolume;
        musicLoopSource.volume = v * musicLoopTrackVolume;
    }
}