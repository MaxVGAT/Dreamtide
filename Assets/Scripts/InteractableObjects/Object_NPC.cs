using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Object_NPC : MonoBehaviour
{
    protected Transform player;
    protected UI ui;

    [SerializeField] private Transform npc;
    [SerializeField] private GameObject interactTooltip;

    [Header("Tooltip Float details")]
    [SerializeField] private float floatSpeed = 2; 
    [SerializeField] private float floatRange = 0.8f; 
    private Vector3 startPosition;

    protected virtual void Awake()
    {
        ui = FindFirstObjectByType<UI>();
        startPosition = interactTooltip.transform.position;
        interactTooltip.SetActive(false);
    }

    protected virtual void Update()
    {
        HandleTooltipFloat();
    }

    private void HandleTooltipFloat()
    {
        if(interactTooltip.activeSelf)
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            interactTooltip.transform.position = startPosition + new Vector3(0, yOffset);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.transform;
        interactTooltip.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        interactTooltip.SetActive(false);
    }
}
