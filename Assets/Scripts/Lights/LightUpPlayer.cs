using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightUpPlayer : MonoBehaviour
{
    private Light2D lampLight;

    [Header("Light details")]
    [SerializeField] private Light2D playerLight;

    [Header("Flickering details")]
    [SerializeField] private float flickerInterval;
    private float targetIntensity;
    private float baseIntensity = 1f;
    private float minFlicker = -0.5f;
    private float maxFlicker = 1f;
    private float changeSpeed = 5f;
    private float timer;

    private void Start()
    {
        lampLight = GetComponentInChildren<Light2D>();

        playerLight.intensity = 0.5f;
        targetIntensity = baseIntensity;
    }

    private void Update()
    {
        SetLightFlicker();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            playerLight.intensity = 1.5f;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            playerLight.intensity = 0.5f;
    }

    private void SetLightFlicker()
    {
        if(timer >= flickerInterval)
        {
            targetIntensity = baseIntensity + Random.Range(minFlicker, maxFlicker);
            flickerInterval = Random.Range(0.05f, 0.3f);
            timer = 0;
        }

        lampLight.intensity = Mathf.Lerp(lampLight.intensity, targetIntensity, Time.deltaTime * changeSpeed);
        timer += Time.deltaTime;
    }


}
