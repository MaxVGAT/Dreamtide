using UnityEngine;

public class UI_MiniHealthBar : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void OnEnable()
    {
        if (entity == null)
            entity = GetComponentInParent<Entity>();

        if (entity != null)
            entity.OnFlipped += HandleMiniHealthBarFlip;
    }

    private void OnDisable()
    {
        if (entity != null)
            entity.OnFlipped -= HandleMiniHealthBarFlip;
    }

    private void HandleMiniHealthBarFlip() => transform.rotation = Quaternion.identity;
}