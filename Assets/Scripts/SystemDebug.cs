using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemDebugger : MonoBehaviour
{
    void Start()
    {
        var allES = Resources.FindObjectsOfTypeAll<EventSystem>();
        Debug.Log($"Total EventSystems in scene (including inactive): {allES.Length}");
        foreach (var es in allES)
        {
            Debug.Log($"{es.name} | ActiveInHierarchy: {es.gameObject.activeInHierarchy} | Scene: {es.gameObject.scene.name}");
        }
    }
}
