using UnityEngine;

public class Entity_SFX : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFX Names")]
    [SerializeField] private string attackHit;
    [SerializeField] private string attackMiss;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void PlayAttackHit()
    {
        SoundManager.instance.PlaySFX(attackHit, audioSource);
    }

    public void PlayAttackMiss()
    {
        SoundManager.instance.PlaySFX(attackMiss, audioSource);
    }
}
