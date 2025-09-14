using UnityEngine;

public class DDOLTracker : MonoBehaviour
{
    void Awake()
    {
        Application.logMessageReceived += (condition, stackTrace, type) =>
        {
            if (condition.Contains("DontDestroyOnLoad"))
            {
                Debug.LogWarning($"DDOL called: {condition}\n{stackTrace}");
            }
        };
    }
}