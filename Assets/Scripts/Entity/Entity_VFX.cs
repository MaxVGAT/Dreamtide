using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{

    private SpriteRenderer sr;
    private Entity entity;


    public enum FlashType { Red, Yellow, Green, White }

    [Header("On Taking Damage VFX")]
    [SerializeField] private Material interactableHitMat;
    [SerializeField] private Material redHitMat;
    [SerializeField] private Material yellowHitBlockMat;
    [SerializeField] private Material greenHitPerfectBlockMat;
    [SerializeField] private float onDamageVfxDuration = 0.2f;
    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;

    [Header("On Doing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;

    [Header("Elements Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color stunVfx = Color.yellow;
    private Color originalHitVfxColor;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
    }

    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Ice)
            StartCoroutine(PlayStatusVfxCo(duration, chillVfx));
    }
    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = 0.25f;
        float timeHasPassed = 0;

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * 0.8f;

        bool toggle = false;

        while(timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);
            timeHasPassed = timeHasPassed + tickInterval;
        }

        sr.color = Color.white;
    }

    public void CreateOnHitVFX(Transform target, bool isCrit)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);
        if(isCrit == false)
            vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;

        if (entity.facingDirection == -1 && isCrit)
            vfx.transform.Rotate(0, 180, 0);
    }

    public void UpdateOnHitColor(ElementType element)
    {
        if (element == ElementType.None)
            hitVfxColor = originalHitVfxColor;

        if (element == ElementType.Ice)
            hitVfxColor = chillVfx;

        if (element == ElementType.Fire)
            hitVfxColor = burnVfx;

        if (element == ElementType.Lightning)
            hitVfxColor = stunVfx;

    }

    public void HandleHitColor(FlashType type)
    {
        Material mat = redHitMat;

        if(type == FlashType.Yellow)
            mat = yellowHitBlockMat;
        if (type == FlashType.White)
            mat = interactableHitMat;
        //else if (type == FlashType.Green)
        //    mat = greenHitPerfectBlockMat;

        PlayOnDamageVfx(mat);
    }

    public void PlayOnDamageVfx(Material hitMaterial)
    {
        if (onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine);

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo(hitMaterial));
    }

    private IEnumerator OnDamageVfxCo(Material hitMaterial)
    {
        sr.material = hitMaterial;

        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial;
    }
}
