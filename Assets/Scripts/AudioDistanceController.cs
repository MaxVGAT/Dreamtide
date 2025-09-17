using UnityEngine;

public class AudioDistanceController : MonoBehaviour
{
    private AudioSource source;
    private Transform player;

    [SerializeField] private float minDistanceToHearSound = 10f;
    [SerializeField] private bool showGizmo;

    private float maxVolume;

    // Flag to ignore distance when NPC is talking
    [HideInInspector] public bool ignoreDistance = false;

    private void Start()
    {
        player = Entity_Player.instance.transform;
        source = GetComponent<AudioSource>();
        maxVolume = source.volume;
    }

    private void Update()
    {
        if (player == null) return;

        float targetVolume = maxVolume;

        if (!ignoreDistance)
        {
            float distance = Vector2.Distance(player.position, transform.position);
            float t = Mathf.Clamp01(1 - (distance / minDistanceToHearSound));
            targetVolume = Mathf.Lerp(0, maxVolume, t * t);
        }
        else
        {
            // Force fully audible
            targetVolume = maxVolume;
        }

        // Use Mathf.Lerp for smooth but faster snap
        source.volume = Mathf.Lerp(source.volume, targetVolume, Time.deltaTime * 20f);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, minDistanceToHearSound);
        }
    }
}
