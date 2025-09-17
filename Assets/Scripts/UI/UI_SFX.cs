using UnityEngine;
using System.Collections;

public class UI_SFX : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("UI SFX Names")]
    [SerializeField] private string hoverSound;
    [SerializeField] private string clickSound;
    [SerializeField] private string startSound;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void PlayHover()
    {
        if (!string.IsNullOrEmpty(hoverSound))
            SoundManager.instance.PlayUISFX(hoverSound, audioSource);
    }

    public void PlayClick()
    {
        if (!string.IsNullOrEmpty(clickSound))
            SoundManager.instance.PlayUISFX(clickSound, audioSource);
    }

    public void PlayStartSFX()
    {
        AudioClip clip = SoundManager.instance.GetClip(startSound);
        if (clip != null)
        {
            StartCoroutine(FadePlaySFX(clip, 1f));
        }

    }

    private IEnumerator FadePlaySFX(AudioClip clip, float duration)
    {
        audioSource.clip = clip;
        audioSource.volume = 1f;
        audioSource.Play();

        float startVol = audioSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVol, 0, t / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVol;
    }
}