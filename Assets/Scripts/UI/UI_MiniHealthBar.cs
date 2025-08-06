using UnityEngine;

public class UI_MiniHealthBar : MonoBehaviour
{

    private Entity entity => GetComponentInParent<Entity>();

    private void OnEnable()
    {
        entity.OnFlipped += HandleMiniHealthBarFlip; // Subscribe to the Event
    }

    private void OnDisable()
    {
        entity.OnFlipped -= HandleMiniHealthBarFlip; // Unsubscribe
    }

    private void HandleMiniHealthBarFlip() => transform.rotation = Quaternion.identity; // Keeps enemies mini health bar at default
}
