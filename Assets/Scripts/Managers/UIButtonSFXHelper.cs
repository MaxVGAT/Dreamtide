using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverSFX : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip hoverSFX;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSFX != null)
            SoundManager.Instance.PlaySFX(hoverSFX);
    }
}