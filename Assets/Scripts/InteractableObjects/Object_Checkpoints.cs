using UnityEngine;

public class Object_Checkpoints : MonoBehaviour, ISaveable
{
    private Object_Checkpoints[] allCheckpoints;
    //private Animator anim; //ADD ANIMATION LATER

    private void Awake()
    {
        //anim = GetComponentInChildren<Animator>();
        allCheckpoints = FindObjectsByType<Object_Checkpoints>(FindObjectsSortMode.None);
    }

    public void ActivateCheckpoint(bool activate)
    {
        //anim.SetBool("isACtive", activate);
        Debug.Log("Activated");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(var point in allCheckpoints)
        {
            point.ActivateCheckpoint(false);

            SaveManager.instance.GetGameData().savedCheckpoint = transform.position;
            ActivateCheckpoint(true);
        }
    }

    public void LoadData(GameData data)
    {
        bool active = data.savedCheckpoint == transform.position;
        ActivateCheckpoint(active);

        if(active)
            Entity_Player.instance.TeleportPlayer(transform.position);
    }

    public void SaveData(ref GameData data)
    {
        
    }
}