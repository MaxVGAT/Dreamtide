using UnityEngine;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Entity_Player player;
    [SerializeField] private Toggle healthBarToggle;

    private void Start()
    {
        player = FindFirstObjectByType<Entity_Player>();
        healthBarToggle.onValueChanged.AddListener(OnHealthBarToggle);
    }

    private void OnHealthBarToggle(bool isOn)
    {
        player.health.EnableHealthBar(isOn);
    }
}
