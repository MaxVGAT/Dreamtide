using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioDatabaseSO audioDB;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private AudioClip lastMusicPlayed;
    private Transform player;
    private string currentBgmGroupName;
    private Coroutine currentBgmCo;
    [SerializeField] private bool bgmShouldPlay;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(bgmSource.isPlaying == false && bgmShouldPlay)
        {
            if (string.IsNullOrEmpty(currentBgmGroupName) == false)
                NextBGM(currentBgmGroupName);
        }

        if (bgmSource.isPlaying && bgmShouldPlay == false)
            StopBGM();
    }

    public void StartBGM(string musicGroup)
    {
        bgmShouldPlay = true;

        if (musicGroup == currentBgmGroupName)
            return;

        NextBGM(musicGroup);
    }

    public void StopBGM()
    {
        bgmShouldPlay = false;

        StartCoroutine(FadeVolumeCo(bgmSource, 0, 1));

        if(currentBgmCo != null)
            StopCoroutine(currentBgmCo);    
    }

    public void NextBGM(string musicGroup)
    {
        bgmShouldPlay = true;
        currentBgmGroupName = musicGroup;

        if(currentBgmCo != null)
            StopCoroutine(currentBgmCo);

        currentBgmCo = StartCoroutine(SwitchMusicCo(musicGroup));
    }

    private IEnumerator SwitchMusicCo(string musicGroup)
    {
        AudioClipData data = audioDB.Get(musicGroup);

        if (data == null || data.clips.Count == 0)
        {
            Debug.Log("No audio found fopr group + musicGroup");
            yield break;
        }

        AudioClip nextMusic = data.GetRandomClip();

        if (data.clips.Count > 1)
        {
            while (nextMusic == lastMusicPlayed)
                nextMusic = data.GetRandomClip();
        }

        if (bgmSource.isPlaying)
            yield return FadeVolumeCo(bgmSource, 0, 1f);

        lastMusicPlayed = nextMusic;
        bgmSource.clip = nextMusic;
        bgmSource.Play();

        StartCoroutine(FadeVolumeCo(bgmSource, data.maxVolume, 1f));
    }

    private IEnumerator FadeVolumeCo(AudioSource source, float targetVolume, float duration)
    {
        float time = 0;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;

            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public void PlaySFX(string soundName, AudioSource sfxSource, bool randomizePitch = true, float minDistanceToHearSound = 5)
    {
        if (player == null)
            player = Entity_Player.instance.transform;

        var data = audioDB.Get(soundName);
        if (data == null) return;

        var clip = data.GetRandomClip();
        if (clip == null) return;

        float maxVolume = data.maxVolume;
        float distance = Vector2.Distance(sfxSource.transform.position, player.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceToHearSound));

        sfxSource.volume = data.maxVolume;
        sfxSource.pitch = randomizePitch ? Random.Range(0.9f, 1.1f) : 1f;
        sfxSource.volume = Mathf.Lerp(0, maxVolume, t * t);

        sfxSource.PlayOneShot(clip);
    }

    public void PlayUISFX(string soundName, AudioSource sfxSource)
    {
        var data = audioDB.Get(soundName);
        if (data == null) return;

        var clip = data.GetRandomClip();
        if (clip == null) return;

        sfxSource.volume = data.maxVolume;
        sfxSource.PlayOneShot(clip);
    }

    public AudioClip GetClip(string soundName)
    {
        var data = audioDB.Get(soundName); // get the AudioDataSO by name
        if (data == null) return null;
        return data.GetRandomClip(); // or first clip if not randomized
    }

    public void PlayNpcSFX(string soundName, AudioSource sfxSource) // REFACTOR THIS ONE WITH THE ONE ABOVE
    {
        var data = audioDB.Get(soundName);
        if (data == null) return;

        var clip = data.GetRandomClip();
        if (clip == null) return;

        sfxSource.volume = data.maxVolume;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayBGMClip(AudioClip clip)
    {
        if (clip == null) return;

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);

        currentBgmCo = StartCoroutine(PlayClipCo(clip));
    }


    private IEnumerator PlayClipCo(AudioClip clip)
    {
        if (bgmSource.isPlaying)
            yield return FadeVolumeCo(bgmSource, 0, 1f);

        bgmSource.clip = clip;
        bgmSource.Play();
        yield return FadeVolumeCo(bgmSource, 1f, 1f); // fade in to full volume
    }

    public void PlayGlobalSFX(string soundName)
    {
        var data = audioDB.Get(soundName);
        if (data == null) return;

        var clip = data.GetRandomClip();
        if (clip == null) return;

        sfxSource.volume = data.maxVolume;
        sfxSource.PlayOneShot(clip, data.maxVolume);
    }
}