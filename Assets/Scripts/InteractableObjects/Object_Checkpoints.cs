using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class Object_Checkpoints : MonoBehaviour, ISaveable
{
    [SerializeField] private string checkpointId;
    [SerializeField] private Transform respawnPoint;
    [Header("Checkpoint Light")]
    [SerializeField] private Light2D checkpointLight;
    [SerializeField] private float baseIntensity = 1f;
    [SerializeField] private float lowerIntensity = 0.5f;
    [SerializeField] private float inactiveIntensity = 0.4f;
    [SerializeField] private float flickerDuration = 1f;

    private static Object_Checkpoints[] allCheckpoints;
    private bool isActive = false;
    private Coroutine flickerRoutine;

    private void Awake()
    {
        if (allCheckpoints == null)
            allCheckpoints = FindObjectsByType<Object_Checkpoints>(FindObjectsSortMode.None);

        if (checkpointLight != null)
        {
            checkpointLight.color = Color.red;
            checkpointLight.intensity = inactiveIntensity;
        }

        // Ensure unique ID
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(checkpointId))
            checkpointId = System.Guid.NewGuid().ToString();
#endif
    }

    public string GetCheckpointId() => checkpointId;
    public Vector3 GetRespawnPosition() => respawnPoint == null ? transform.position : respawnPoint.position;

    public void ActivateCheckpoint(bool activate)
    {
        if (activate)
        {
            // Deactivate all others
            foreach (var cp in allCheckpoints)
            {
                if (cp != this)
                    cp.ActivateCheckpoint(false);
            }
        }

        isActive = activate;

        if (checkpointLight != null)
        {
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
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            float timer = 0f;

            while (timer < flickerDuration)
            {
                checkpointLight.intensity = Mathf.Lerp(lowerIntensity, baseIntensity, timer / flickerDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            checkpointLight.intensity = baseIntensity;
            timer = 0f;

            while (timer < flickerDuration)
            {
                checkpointLight.intensity = Mathf.Lerp(baseIntensity, lowerIntensity, timer / flickerDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            checkpointLight.intensity = lowerIntensity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        ActivateCheckpoint(true);
    }

    // -------------------------
    // ISaveable implementation
    // -------------------------
    public void LoadData(GameData data)
    {
        if (data.unlockedCheckpoints.TryGetValue(checkpointId, out bool unlocked) && unlocked)
        {
            ActivateCheckpoint(true);
        }
        else
        {
            ActivateCheckpoint(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (!isActive) return;

        if (!data.unlockedCheckpoints.ContainsKey(checkpointId))
            data.unlockedCheckpoints.Add(checkpointId, true);
    }
}
