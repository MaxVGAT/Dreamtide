using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class Object_Checkpoints : MonoBehaviour, ISaveable
{
    private Object_Checkpoints[] allCheckpoints;

    [Header("Checkpoint Light")]
    [SerializeField] private Light2D checkpointLight; // assign the light in Inspector
    [SerializeField] private float baseIntensity = 1f;
    [SerializeField] private float lowerIntensity = 0.5f;
    [SerializeField] private float inactiveIntensity = 0.4f;
    [SerializeField] private float duration;


    private bool isActive = false;
    private Coroutine flickerRoutine;

    private void Awake()
    {
        allCheckpoints = FindObjectsByType<Object_Checkpoints>(FindObjectsSortMode.None);

        if (checkpointLight != null)
        {
            checkpointLight.color = Color.red;
            checkpointLight.intensity = inactiveIntensity; // start off
        }
    }


    public void ActivateCheckpoint(bool activate)
    {
        isActive = activate;

        if (checkpointLight == null)
            return;

        if (activate)
        {
            checkpointLight.color = Color.green;
            checkpointLight.intensity = lowerIntensity;
            flickerRoutine = StartCoroutine(FlickerLight());
        }
        else
        {
            checkpointLight.color = Color.red;
            checkpointLight.intensity = 0f;
            if (flickerRoutine != null)
                StopCoroutine(flickerRoutine);
        }
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            float timer = 0f;
            float startIntensity = baseIntensity;

            while (timer < duration)
            {
                checkpointLight.intensity = Mathf.Lerp(lowerIntensity, baseIntensity, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            checkpointLight.intensity = baseIntensity;

            timer = 0f;
            while(timer < duration)
            {
                checkpointLight.intensity = Mathf.Lerp(baseIntensity, lowerIntensity, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            checkpointLight.intensity = lowerIntensity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        foreach (var point in allCheckpoints)
        {
            if (point != this)
                point.ActivateCheckpoint(false);
        }
        ActivateCheckpoint(true);

        // Save checkpoint
        //SaveManager.instance.GetGameData().savedCheckpoint = transform.position;
    }

    public void LoadData(GameData data)
    {
        bool active = data.savedCheckpoint == transform.position;
        ActivateCheckpoint(active);

        if (active)
            Entity_Player.instance.TeleportPlayer(transform.position);
    }

    public void SaveData(ref GameData data)
    {
        // You can store checkpoint activation here if needed
    }
}
