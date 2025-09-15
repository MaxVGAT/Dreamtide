//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Rendering.Universal;
//using System.Collections;

//public class Object_Checkpoints : MonoBehaviour, ISaveable
//{
//    [SerializeField] private string checkpointId;
//    [SerializeField] private Transform respawnPoint;
//    [Header("Checkpoint Light")]
//    [SerializeField] private Light2D checkpointLight;
//    [SerializeField] private float baseIntensity = 1f;
//    [SerializeField] private float lowerIntensity = 0.5f;
//    [SerializeField] private float inactiveIntensity = 0.4f;
//    [SerializeField] private float flickerDuration = 1f;

//    private static Object_Checkpoints[] allCheckpoints;
//    private bool isActive = false;
//    private bool isUnlocked = false;
//    private Coroutine flickerRoutine;

//    private void Awake()
//    {
//        if (allCheckpoints == null)
//            allCheckpoints = FindObjectsByType<Object_Checkpoints>(FindObjectsSortMode.None);

//        if (checkpointLight != null)
//        {
//            checkpointLight.color = Color.red;
//            checkpointLight.intensity = inactiveIntensity;
//        }

//        // Ensure unique ID
//#if UNITY_EDITOR
//        if (string.IsNullOrEmpty(checkpointId))
//            checkpointId = System.Guid.NewGuid().ToString();
//#endif

//        // Don't activate here; GameManager will handle it after scene load
//    }

//    public string GetCheckpointId() => checkpointId;
//    public Vector3 GetRespawnPosition() => respawnPoint == null ? transform.position : respawnPoint.position;

//    public void ActivateCheckpoint(bool activate)
//    {
//        if (activate)
//        {
//            // Mark as unlocked when activated
//            isUnlocked = true;

//            // Let GameManager handle the logic - don't deactivate others here
//            if (GameManager.instance != null)
//            {
//                // Just notify GameManager, let it handle the rest
//                GameManager.instance.UnlockAndSetCheckpoint(checkpointId);
//                Debug.Log($"[Checkpoint] Notified GameManager about checkpoint: {checkpointId}");
//            }
//        }

//        SetActiveState(activate);
//    }

//    // Separate method for just setting the visual state without triggering GameManager
//    public void SetActiveState(bool active)
//    {
//        isActive = active;

//        if (checkpointLight != null)
//        {
//            if (active && isUnlocked)
//            {
//                checkpointLight.color = Color.green;
//                checkpointLight.intensity = lowerIntensity;
//                if (flickerRoutine != null)
//                    StopCoroutine(flickerRoutine);
//                flickerRoutine = StartCoroutine(FlickerLight());
//            }
//            else if (isUnlocked)
//            {
//                // Unlocked but not active - dim green/yellow
//                checkpointLight.color = Color.yellow;
//                checkpointLight.intensity = inactiveIntensity;
//                if (flickerRoutine != null)
//                    StopCoroutine(flickerRoutine);
//            }
//            else
//            {
//                // Not unlocked - red
//                checkpointLight.color = Color.red;
//                checkpointLight.intensity = inactiveIntensity;
//                if (flickerRoutine != null)
//                    StopCoroutine(flickerRoutine);
//            }
//        }
//    }

//    private IEnumerator FlickerLight()
//    {
//        while (isActive && isUnlocked)
//        {
//            float timer = 0f;

//            while (timer < flickerDuration && isActive)
//            {
//                if (checkpointLight != null)
//                    checkpointLight.intensity = Mathf.Lerp(lowerIntensity, baseIntensity, timer / flickerDuration);
//                timer += Time.deltaTime;
//                yield return null;
//            }

//            if (checkpointLight != null)
//                checkpointLight.intensity = baseIntensity;
//            timer = 0f;

//            while (timer < flickerDuration && isActive)
//            {
//                if (checkpointLight != null)
//                    checkpointLight.intensity = Mathf.Lerp(baseIntensity, lowerIntensity, timer / flickerDuration);
//                timer += Time.deltaTime;
//                yield return null;
//            }

//            if (checkpointLight != null)
//                checkpointLight.intensity = lowerIntensity;
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (!collision.CompareTag("Player")) return;

//        ActivateCheckpoint(true);
//        Debug.Log($"[Checkpoint] Player triggered checkpoint: {checkpointId}");
//    }

//    // -------------------------
//    // ISaveable implementation - SIMPLIFIED
//    // -------------------------
//    public void LoadData(GameData data)
//    {
//        // Check if this checkpoint is unlocked
//        if (data.unlockedCheckpoints.TryGetValue(checkpointId, out bool unlocked) && unlocked)
//        {
//            isUnlocked = true;
//        }
//        else
//        {
//            isUnlocked = false;
//        }

//        // Don't set active state here - let GameManager handle it
//        // Just set the visual state based on unlock status
//        if (isUnlocked)
//        {
//            checkpointLight.color = Color.yellow;
//            checkpointLight.intensity = inactiveIntensity;
//        }
//        else
//        {
//            checkpointLight.color = Color.red;
//            checkpointLight.intensity = inactiveIntensity;
//        }

//        Debug.Log($"[Checkpoint] {checkpointId} loaded - unlocked: {isUnlocked}");
//    }

//    public void SaveData(ref GameData data)
//    {
//        // Only save if unlocked - let GameManager handle active checkpoint logic
//        if (isUnlocked)
//        {
//            data.unlockedCheckpoints[checkpointId] = true;
//        }
//    }
//}