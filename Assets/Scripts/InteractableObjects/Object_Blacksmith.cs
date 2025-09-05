using UnityEngine;

public class Object_Blacksmith : Object_NPC, IInteractable
{
    public void Interact()
    {
        Debug.Log("Craft!");
    }
}
