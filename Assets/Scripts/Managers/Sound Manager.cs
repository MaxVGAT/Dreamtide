using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioDatabaseSO audioDB;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

    }

    public void PlaySFX(string soundName, AudioSource sfxSource)
    {
        var data = audioDB.Get(soundName);
        if (data == null)
        {
            Debug.Log("Attempt - " + soundName);
            return;
        }

        var clip = data.GetRandomClip();
        if (clip == null)
            return;

        sfxSource.clip = clip;
        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        sfxSource.volume = data.volume;
        sfxSource.PlayOneShot(clip);
        Debug.Log(clip + " - Played");
    }
}
