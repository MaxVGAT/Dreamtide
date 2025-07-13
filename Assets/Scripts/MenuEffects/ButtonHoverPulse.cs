using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverPulse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("Hover Settings")]
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] private float pulseSpeed = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource SFXSource;

    private Vector3 originalScale;
    private bool isHovering = false;

    //==================================================
    // UI HOVER EFFECT
    //==================================================

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        Vector3 targetScale = isHovering ? new Vector3(originalScale.x * hoverScale.x, originalScale.y * hoverScale.y, originalScale.z * hoverScale.z) : originalScale;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * pulseSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        // Play hover SFX
        if (SFXSource != null)
        {
            SoundManager.Instance?.PlayHoverSFX();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        isHovering = true;

        // Play hover SFX
        if (SFXSource != null)
        {
            SoundManager.Instance?.PlayHoverSFX();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isHovering = false;
    }
}
