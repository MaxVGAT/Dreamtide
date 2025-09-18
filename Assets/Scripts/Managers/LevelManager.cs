using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private AudioClip levelBGM; // assign in inspector

    private void Start()
    {
        SoundManager.instance.PlayBGMClip(levelBGM);
    }
}
