using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverSFX : MonoBehaviour, IPointerEnterHandler
{
    public UI_SFX uiSFX;

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiSFX?.PlayHover();
    }
}