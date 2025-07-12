using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIClickDebugger : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(eventSystem);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    Debug.Log("Clicked on UI: " + result.gameObject.name);
                }
            }
            else
            {
                Debug.Log("Clicked on UI: Nothing");
            }
        }
    }
}