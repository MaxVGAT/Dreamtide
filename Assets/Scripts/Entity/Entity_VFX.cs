using System.Collections;
using UnityEngine;

// �G���e�B�e�B�̊e��VFX�i�_���[�W�A�X�e�[�^�X�A�U�����j��Ǘ�����N���X
public class Entity_VFX : MonoBehaviour
{
    protected SpriteRenderer sr; // �X�v���C�g�`��p
    private Entity entity;

    public enum FlashType { Red, Yellow, Green, White } // �_���[�W�t���b�V���^�C�v

    [Header("On Taking Damage VFX")]
    [SerializeField] private Material interactableHitMat;
    [SerializeField] private Material redHitMat;
    [SerializeField] private Material yellowHitBlockMat;
    [SerializeField] private Material greenHitPerfectBlockMat;
    [SerializeField] private float onDamageVfxDuration = 0.2f;
    private Material originalMaterial; // ���̃}�e���A����ێ�
    private Coroutine onDamageVfxCoroutine;

    [Header("On Doing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;

    [Header("Elements Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color shockVfx = Color.yellow;
    private Color originalHitVfxColor;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material; // �����}�e���A���ۑ�
        originalHitVfxColor = hitVfxColor;
    }

    // �X�e�[�^�X���ʂɉ�����VFX��Đ�
    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        if (element == ElementType.Ice)
            StartCoroutine(PlayStatusVfxCo(duration, chillVfx));

        if (element == ElementType.Fire)
            StartCoroutine(PlayStatusVfxCo(duration, burnVfx));

        if (element == ElementType.Lightning)
            StartCoroutine(PlayStatusVfxCo(duration, shockVfx));
    }

    // �SVFX��~�i�t���b�V����X�e�[�^�X�F����Z�b�g�j
    public void StopAllVfx()
    {
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;
    }

    // �X�e�[�^�XVFX�̃R���[�`���i�F�̓_�Łj
    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = 0.25f; // �_�ŊԊu
        float timeHasPassed = 0;

        Color lightColor = effectColor * 1.2f; // ���邢�F
        Color darkColor = effectColor * 0.8f;   // �Â��F

        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor; // ��݂ɐF�ύX
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);
            timeHasPassed += tickInterval;
        }

        sr.color = Color.white; // �I�����Ɍ��F��
    }

    // �U����VFX�����i�ʏ� or �N���e�B�J���j
    public void CreateOnHitVFX(Transform target, bool isCrit, ElementType element)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);

        // ���]�����i�������̂Ƃ��N���e�B�J��VFX�𔽓]�j
        if (entity.facingDirection == -1 && isCrit)
            vfx.transform.Rotate(0, 180, 0);
    }

    // �����ɉ�����VFX�F��擾
    public Color GetElementColor(ElementType element)
    {
        switch (element)
        {
            case ElementType.Ice: return chillVfx;
            case ElementType.Fire: return burnVfx;
            case ElementType.Lightning: return shockVfx;
            default: return Color.white;
        }
    }

    // �_���[�W���̐F�t���b�V��
    public void HandleHitColor(FlashType type)
    {
        Material mat = redHitMat;

        if (type == FlashType.Yellow)
            mat = yellowHitBlockMat;
        if (type == FlashType.White)
            mat = interactableHitMat;
        //else if (type == FlashType.Green)
        //    mat = greenHitPerfectBlockMat;

        PlayOnDamageVfx(mat);
    }

    // �_���[�WVFX�Đ��i�}�e���A���ύX�j
    public void PlayOnDamageVfx(Material hitMaterial)
    {
        if (onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine); // �����R���[�`����~

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo(hitMaterial));
    }

    // �_���[�WVFX�R���[�`���i�w�莞�Ԃ����t���b�V���j
    private IEnumerator OnDamageVfxCo(Material hitMaterial)
    {
        sr.material = hitMaterial; // �t���b�V���p�}�e���A���ɕύX

        yield return new WaitForSeconds(onDamageVfxDuration);

        sr.material = originalMaterial; // �I�����Ɍ��}�e���A���֖߂�
    }
}
