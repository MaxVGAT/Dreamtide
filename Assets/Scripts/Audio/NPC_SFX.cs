using UnityEngine;

public class NPC_SFX : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFX Names")]
    [SerializeField] private string talkSfx;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void PlayTalkSfx()
    {
        SoundManager.instance.PlayNpcSFX(talkSfx, audioSource);
    }
}
