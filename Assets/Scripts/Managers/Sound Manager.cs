using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // Singleton instance for global access
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("Musics")]
    [SerializeField] private AudioClip titleMusic;
    [SerializeField] private AudioClip ambientMusic;

    [Header("Menu SFX")]
    [SerializeField] private AudioClip buttonPressSFX;
    [SerializeField] private AudioClip buttonCloseSFX;
    [SerializeField] public AudioClip hoverSFX;
    [SerializeField] private AudioClip startSFX;
    [SerializeField] private AudioClip PauseSFX;

    [Header("Sound Settings")]
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.1f;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.1f;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private bool isMuted;

    private void Awake()
    {
        // Singleton pattern enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved volume settings
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);

        if (musicSlider != null)
        {
            musicSlider.value = musicVolume; // Force the slider to match loaded volume
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    private void Start()
    {
        // Initialize music for current scene
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Play appropriate music based on loaded scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            
            case "MainMenu":
                PlayMusic(titleMusic);
                PlayAmbient(ambientMusic);
                break;
            //case "InGame":
            //    PlayMusic(lobbyMusic);
            //    break;
        }
    }

    // Play a sound effect once at specified volume
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // Play or switch music, avoid restarting same clip
    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (musicSource.clip == musicClip && musicSource.isPlaying) return;

        musicSource.clip = musicClip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlayAmbient(AudioClip ambientClip, bool loop = true)
    {
        if (ambientSource.clip == ambientClip && ambientSource.isPlaying) return;

        ambientSource.clip = ambientClip;
        ambientSource.loop = loop;
        ambientSource.volume = musicVolume;
        ambientSource.Play();
    }

    // Convenience methods for specific SFX
    public void PlayOpenButtonSFX() => PlaySFX(buttonPressSFX);

    public void PlayCloseButtonSFX() => PlaySFX(buttonCloseSFX);

    public void PlayHoverSFX() => PlaySFX(hoverSFX);

    public void PlayPauseSFX() => PlaySFX(PauseSFX);

    public void PlayStartSFX() => PlaySFX(startSFX);

    // Set music volume and save preference
    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("MusicVol", VolumeToDB(volume));
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("SFXVol", VolumeToDB(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);  
    }

    float VolumeToDB(float volume)
    {
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
    }
}
