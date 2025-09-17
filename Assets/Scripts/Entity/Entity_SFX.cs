using UnityEngine;

public class Entity_SFX : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFX Names")]
    [SerializeField] private string attackHit;
    [SerializeField] private string attackMiss;
    [Space]
    [SerializeField] private float soundDistance = 10f;
    [SerializeField] private bool showGizmo;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void PlayAttackHit()
    {
        SoundManager.instance.PlaySFX(attackHit, audioSource, true, soundDistance);
    }

    public void PlayAttackMiss()
    {
        SoundManager.instance.PlaySFX(attackMiss, audioSource, true, soundDistance);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, soundDistance);

        }
    }
}
