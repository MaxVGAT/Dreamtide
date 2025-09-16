using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Fade : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    public Coroutine fadeEffectCo { get; private set; }

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 1); // Start fully black
    }

    // Coroutine versions you can yield
    public IEnumerator DoFadeInCo(float duration = 1f)  // Black -> Transparent
    {
        yield return FadeEffectCo(0f, duration);
    }

    public IEnumerator DoFadeOutCo(float duration = 1f) // Transparent -> Black
    {
        yield return FadeEffectCo(1f, duration);
    }

    // Simple wrapper if you just want to fire-and-forget
    public void DoFadeIn(float duration = 1f) => FadeEffect(0f, duration);
    public void DoFadeOut(float duration = 1f) => FadeEffect(1f, duration);

    private void FadeEffect(float targetAlpha, float duration)
    {
        if (fadeEffectCo != null)
            StopCoroutine(fadeEffectCo);
        fadeEffectCo = StartCoroutine(FadeEffectCo(targetAlpha, duration));
    }

    private IEnumerator FadeEffectCo(float targetAlpha, float duration)
    {
        if (fadeImage == null) yield break;

        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);

        // Clear the coroutine reference when done
        fadeEffectCo = null;
    }

    public IEnumerator FadeOutCoroutine(float duration = 1f)
    {
        DoFadeOut(duration);
        while (fadeEffectCo != null)
            yield return null;
    }
}