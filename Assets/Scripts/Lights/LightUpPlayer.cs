using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightUpPlayer : MonoBehaviour
{
    [Header("Lamp light")]
    private Light2D lampLight;                 // The lamp’s own light
    [SerializeField] private Light2D playerLight; // Player’s light

    [Header("Flicker settings")]
    [SerializeField] private float baseIntensity = 1f;
    [SerializeField] private float minFlicker = -0.5f;
    [SerializeField] private float maxFlicker = 1f;
    [SerializeField] private float changeSpeed = 5f;

    private float targetIntensity;
    private float flickerTimer;
    private float flickerInterval;
    private bool playerInside = false;

    private void Awake()
    {
        lampLight = GetComponentInChildren<Light2D>();
        targetIntensity = baseIntensity;
        flickerInterval = Random.Range(0.05f, 0.3f);
    }

    private void Update()
    {
        FlickerLamp();

        // Only update player light if inside trigger
        if (playerInside && playerLight != null)
            playerLight.intensity = lampLight.intensity;
    }

    private void FlickerLamp()
    {
        flickerTimer += Time.deltaTime;

        if (flickerTimer >= flickerInterval)
        {
            targetIntensity = baseIntensity + Random.Range(minFlicker, maxFlicker);
            flickerInterval = Random.Range(0.05f, 0.3f);
            flickerTimer = 0f;
        }

        lampLight.intensity = Mathf.Lerp(lampLight.intensity, targetIntensity, Time.deltaTime * changeSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = false;
    }
}
