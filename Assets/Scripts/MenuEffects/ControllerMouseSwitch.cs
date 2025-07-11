using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerMouseSwitch : MonoBehaviour
{
    public GameObject firstSelected;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}
