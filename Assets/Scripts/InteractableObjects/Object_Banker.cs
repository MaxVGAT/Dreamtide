using UnityEngine;

public class Object_Banker : Object_NPC, IInteractable
{
    public void Interact()
    {
        Debug.Log("Open bank!");
    }
}
